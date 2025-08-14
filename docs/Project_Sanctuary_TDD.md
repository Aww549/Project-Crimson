# Technical Design Document: Project Sanctuary

## 1. New Scripts & Their Responsibilities

To build this system, we will need the following new C# scripts:

*   **`SanctuaryManager.cs` (Singleton MonoBehaviour)**
    *   **Responsibilities:** This will be the central controller for the player's base. It will persist across scenes.
        *   Manages the master list of all `Survivor` objects the player owns.
        *   Tracks the current level of each of the three main buildings (Armory, Hospital, Workshop).
        *   Manages the new "Materials" currency (e.g., `int currentMaterials`).
        *   Provides public API methods like `AddMaterials(int amount)`, `UpgradeBuilding(BuildingType type)`, and `GetIdleSurvivors()`.
        *   Interfaces directly with `GameDataManager` to trigger saving and loading of all Sanctuary-related data.

*   **`MissionController.cs` (Singleton MonoBehaviour)**
    *   **Responsibilities:** This script will handle the logic for all off-screen missions.
        *   Manages a list of currently active missions (`List<Mission>`).
        *   Handles the core logic of starting a mission: assigning survivors, calculating success chance based on the number of participants, and setting a real-time timer.
        *   Checks for completed missions upon loading the game and after a mission's timer has elapsed.
        *   Processes mission outcomes: distributing "Materials" on success, and handling survivor death on failure.

*   **`Survivor.cs` (Plain C# Class)**
    *   **Responsibilities:** This is a data-only class that defines a survivor. It will not be a `MonoBehaviour`.
        *   Contains all data for a single survivor, such as a unique ID, name, and current status.
        *   It will be marked as `[System.Serializable]` to allow `GameDataManager` to easily save it.
        *   Designed to be easily expanded with new properties like `Traits` in the future.

*   **`SanctuaryUI.cs` (MonoBehaviour)**
    *   **Responsibilities:** This script will manage all UI elements on the new "Sanctuary" screen.
        *   Renders the list of available survivors.
        *   Displays current building levels and the cost for the next upgrade.
        *   Shows available and in-progress missions, including their remaining time.
        *   Listens for player input to assign survivors and start missions, calling the appropriate methods in `SanctuaryManager` and `MissionController`.

*   **`HelplessSurvivorPickup.cs` (MonoBehaviour)**
    *   **Responsibilities:** This script will be attached to the new "Helpless Survivor" prefab that appears during a run.
        *   On collision with the player, it will not add a survivor directly. Instead, it will set a temporary flag in a persistent script (e.g., `GameDataManager.Instance.SurvivorRescuedThisRun = true;`).
        *   The actual survivor will be added to the `SanctuaryManager`'s roster via the `GameDataManager` only after the run successfully concludes, preventing loss of the survivor if the player dies.

## 2. Data Structures

We need robust, serializable data structures for survivors and missions.

**Survivor Data Structure:**

```csharp
[System.Serializable]
public class Survivor
{
    public string survivorId;      // A unique ID, e.g., generated via System.Guid.NewGuid()
    public string survivorName;    // A randomly generated name, e.g., "Survivor #42"
    public SurvivorStatus status;  // An enum to track what the survivor is doing
    public string assignedMissionId; // ID of the mission they are on, if any

    // Future-proofing for Survivor Traits
    // public List<SurvivorTrait> traits;

    public Survivor()
    {
        survivorId = System.Guid.NewGuid().ToString();
        status = SurvivorStatus.Idle;
        survivorName = "Survivor " + UnityEngine.Random.Range(100, 999);
    }
}

public enum SurvivorStatus { Idle, OnMission, Wounded }
```

**Active Mission Data Structure:**

```csharp
[System.Serializable]
public class ActiveMission
{
    public string missionId;
    public long missionEndTimeTicks; // Store the end time using DateTime.UtcNow.Ticks for time-zone-safe offline tracking
    public List<string> assignedSurvivorIds;
    public float successChance;
}
```

## 3. Data Persistence (Changes to `GameDataManager.cs`)

`GameDataManager.cs` must be updated to save and load our new data structures.

The `GameData` class within `GameDataManager.cs` will be expanded to include:

```csharp
// Inside the GameData class in GameDataManager.cs
[System.Serializable]
public class GameData
{
    // ... existing data like totalScrap, upgrade levels ...

    public int materials;
    public List<Survivor> sanctuarySurvivors;
    public List<ActiveMission> activeMissions;

    public int armoryLevel;
    public int hospitalLevel;
    public int workshopLevel;

    // A flag to ensure a rescued survivor isn't lost if the player dies mid-run
    public bool survivorRescuedThisRun;
}
```

The `SaveGame()` and `LoadGame()` methods will need to be updated to serialize and deserialize this new `GameData` object. The `OnDestroy()` method in `PlayerManager` will be modified to set `survivorRescuedThisRun = false` and, if it was true, add a new `Survivor` to the `GameData` list before saving.

## 4. Script Interaction Diagram

Here is the flow of communication for a few key scenarios:

1.  **Rescuing a Survivor:**
    *   `Player` collides with `HelplessSurvivorPickup` -> `HelplessSurvivorPickup` sets `GameDataManager.Instance.survivorRescuedThisRun = true;` -> `PlayerManager.OnDestroy()` is called at the end of the run -> `GameDataManager` checks the flag, adds a `new Survivor()` to its data, and saves the game.

2.  **Starting a Mission:**
    *   `Player` clicks "Start Mission" button in `SanctuaryUI` -> `SanctuaryUI` calls `MissionController.Instance.StartMission(selectedSurvivorIds)` -> `MissionController` creates a new `ActiveMission` object, sets the status of the assigned `Survivor` objects to `OnMission`, and calculates the `missionEndTimeTicks` -> `MissionController` tells `GameDataManager.Instance.SaveGame()` to persist this new state.

3.  **Checking Mission Timers:**
    *   `GameDataManager.LoadGame()` is called on game start -> `MissionController` receives the list of `activeMissions` -> It iterates through the list, comparing `DateTime.UtcNow.Ticks` with each mission's `missionEndTimeTicks` -> If a mission is complete, `MissionController` processes the result (rewards/losses) and updates the data in `SanctuaryManager` and `GameDataManager`.

## 5. Biggest Technical Challenges

1.  **Reliable Offline Timers:** The biggest challenge is ensuring mission timers progress accurately even when the player is offline.
    *   **Solution:** We must not use `Time.deltaTime` or any in-game clock. When a mission starts, we must save a future timestamp to the device. The correct approach is to store `DateTime.UtcNow.Ticks` for the mission's end time. When the game loads, we get the current `DateTime.UtcNow.Ticks` and compare it to the saved end time to see if the mission is complete. This is robust and not vulnerable to players changing their device's clock.

2.  **Complex Data Interdependencies & "Source of Truth":** With survivors, buildings, and missions, the game state becomes much more complex. A single action (like a mission failing) can trigger multiple data changes (survivor removed, UI updated, data saved).
    *   **Solution:** We must enforce a strict "source of truth" principle. The `GameDataManager`'s `GameData` object should be the absolute source of truth. All other scripts (like `SanctuaryManager` or `MissionController`) should load their state from it at startup and immediately write back any changes. This prevents data from getting out of sync between different managers or game sessions. For example, `SanctuaryManager` should *never* have its own separate list of survivors; it should always reference the list within `GameDataManager`'s data.
