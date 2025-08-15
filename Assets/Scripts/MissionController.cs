using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class MissionController : MonoBehaviour
{
    public static MissionController Instance { get; private set; }

    // We will store our MissionData assets in a Resources folder
    // so the controller can load them automatically.
    private List<MissionData> allPossibleMissions;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllMissions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Check for completed missions when the game starts up.
        CheckForCompletedMissions();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Also check when the player returns to the game.
        if (hasFocus)
        {
            CheckForCompletedMissions();
        }
    }

    /// <summary>
    /// Loads all MissionData assets from the "Resources/Missions" folder.
    /// </summary>
    private void LoadAllMissions()
    {
        allPossibleMissions = Resources.LoadAll<MissionData>("Missions").ToList();
        Debug.Log($"Loaded {allPossibleMissions.Count} missions from Resources.");
    }

    /// <summary>
    /// The main public function to start a new mission.
    /// </summary>
    public void StartMission(MissionData missionData, List<Survivor> assignedSurvivors)
    {
        // --- 1. Calculate Final Success Chance ---
        float finalSuccessChance = missionData.baseSuccessChance;
        // Add bonus for extra survivors
        if (assignedSurvivors.Count > 1)
        {
            finalSuccessChance += (assignedSurvivors.Count - 1) * missionData.bonusSuccessChancePerSurvivor;
        }
        // Apply trait modifiers from all survivors
        foreach (var survivor in assignedSurvivors)
        {
            foreach (var trait in survivor.traits)
            {
                finalSuccessChance += trait.successChanceModifier;
            }
        }
        finalSuccessChance = Mathf.Clamp01(finalSuccessChance); // Ensure it's between 0 and 1

        // --- 2. Create ActiveMission Object ---
        ActiveMission newMission = new ActiveMission();
        newMission.missionId = Guid.NewGuid().ToString();
        newMission.missionDataName = missionData.name; // Save the asset name
        newMission.assignedSurvivorIds = assignedSurvivors.Select(s => s.survivorId).ToList();
        newMission.finalSuccessChance = finalSuccessChance;

        // --- 3. Calculate End Time ---
        double durationInSeconds = missionData.durationHours * 3600;
        DateTime endTime = DateTime.UtcNow.AddSeconds(durationInSeconds);
        newMission.missionEndTimeTicks = endTime.Ticks;

        // --- 4. Update Survivor Status & Save ---
        foreach (var survivor in assignedSurvivors)
        {
            survivor.status = SurvivorStatus.OnMission;
            survivor.assignedMissionId = newMission.missionId;
        }

        GameDataManager.Instance.gameData.activeMissions.Add(newMission);
        GameDataManager.Instance.SaveGame();

        Debug.Log($"Started mission '{missionData.missionName}' with {assignedSurvivors.Count} survivors. Success chance: {finalSuccessChance * 100}%. Ends at: {endTime}");
    }

    /// <summary>
    /// Checks all active missions to see if their timer has expired.
    /// </summary>
    private void CheckForCompletedMissions()
    {
        // We iterate backwards because we might remove items from the list.
        for (int i = GameDataManager.Instance.gameData.activeMissions.Count - 1; i >= 0; i--)
        {
            var mission = GameDataManager.Instance.gameData.activeMissions[i];
            if (DateTime.UtcNow.Ticks >= mission.missionEndTimeTicks)
            {
                ProcessMissionResult(mission);
            }
        }
    }

    /// <summary>
    /// Processes the result of a completed mission (success or failure).
    /// </summary>
    private void ProcessMissionResult(ActiveMission mission)
    {
        // Find the original MissionData asset
        MissionData missionData = allPossibleMissions.FirstOrDefault(m => m.name == mission.missionDataName);
        if (missionData == null)
        {
            Debug.LogError($"Could not find MissionData for completed mission: {mission.missionDataName}");
            return;
        }

        bool wasSuccessful = UnityEngine.Random.value < mission.finalSuccessChance;

        if (wasSuccessful)
        {
            // --- SUCCESS ---
            int rewardAmount = missionData.baseRewardAmount; // We can apply reward traits here later
            if (missionData.rewardType == MissionRewardType.Materials)
            {
                GameDataManager.Instance.gameData.materials += rewardAmount;
            }
            else
            {
                GameDataManager.Instance.AddScrap(rewardAmount);
            }
            Debug.Log($"Mission '{missionData.missionName}' SUCCEEDED! Player earned {rewardAmount} {missionData.rewardType}.");
        }
        else
        {
            // --- FAILURE ---
            Debug.Log($"Mission '{missionData.missionName}' FAILED...");
            // We'll add survivor death logic here later.
        }

        // --- Cleanup ---
        foreach (var survivorId in mission.assignedSurvivorIds)
        {
            var survivor = GameDataManager.Instance.gameData.sanctuarySurvivors.FirstOrDefault(s => s.survivorId == survivorId);
            if (survivor != null)
            {
                survivor.status = SurvivorStatus.Idle;
                survivor.assignedMissionId = string.Empty;
            }
        }

        GameDataManager.Instance.gameData.activeMissions.Remove(mission);
        GameDataManager.Instance.SaveGame();
    }
}
