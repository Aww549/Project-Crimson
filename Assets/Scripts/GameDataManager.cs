using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

[Serializable]
public class ActiveMission
{
    public string missionId;
    public string missionDataName;
    public List<string> assignedSurvivorIds;
    public long missionEndTimeTicks;
    public float finalSuccessChance;

    // New for Claim flow
    public bool isReadyToClaim;
}

[Serializable]
public class GameData
{
    public int totalScrap;
    public int damageUpgradeLevel;
    public int startingSurvivorsUpgradeLevel;
    public int scrapValueUpgradeLevel;
    public int firstAidKitLevel;
    public int materials;
    public List<Survivor> sanctuarySurvivors;
    public List<ActiveMission> activeMissions;
    public int armoryLevel;
    public int hospitalLevel;
    public int workshopLevel;
    public int survivorsRescuedThisRun;
    public float survivorPityChance;

    public string rescuedSurvivorArchetypeName;

    public GameData()
    {
        totalScrap = 0;
        damageUpgradeLevel = 1;
        startingSurvivorsUpgradeLevel = 1;
        scrapValueUpgradeLevel = 1;
        firstAidKitLevel = 0;
        materials = 0;
        sanctuarySurvivors = new List<Survivor>();
        activeMissions = new List<ActiveMission>();
        armoryLevel = 1;
        hospitalLevel = 1;
        workshopLevel = 1;
        survivorsRescuedThisRun = 0;
        survivorPityChance = 0f;
        rescuedSurvivorArchetypeName = null;
    }
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }
    public GameData gameData;
    private string saveFilePath;

    private const int BASE_UPGRADE_COST = 50;
    private const int COST_INCREASE_PER_LEVEL = 25;
    private const int SHIELD_BASE_COST = 250;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetRescuedSurvivor(SurvivorArchetype archetype)
    {
        gameData.survivorsRescuedThisRun++;
        if (archetype != null)
        {
            gameData.rescuedSurvivorArchetypeName = archetype.name;
        }
    }

    // Persistence
    #region
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData();
        }

        PostLoadFixups();
    }

    private void PostLoadFixups()
    {
        if (gameData == null)
        {
            gameData = new GameData();
            return;
        }

        if (gameData.sanctuarySurvivors == null)
            gameData.sanctuarySurvivors = new List<Survivor>();
        if (gameData.activeMissions == null)
            gameData.activeMissions = new List<ActiveMission>();

        foreach (var s in gameData.sanctuarySurvivors)
        {
            if (s == null) continue;
            if (s.traits == null) s.traits = new List<Trait>();
            if (s.assignedMissionId == null) s.assignedMissionId = string.Empty;
            if (string.IsNullOrEmpty(s.survivorId)) s.survivorId = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(s.survivorName)) s.survivorName = "Unnamed";
        }

        foreach (var m in gameData.activeMissions)
        {
            if (m == null) continue;
            if (m.assignedSurvivorIds == null) m.assignedSurvivorIds = new List<string>();
            // Default for older saves
            // If the field didn't exist, it will be 'false' by default anyway.
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);
    }
    #endregion

    // Upgrades and helpers
    #region
    public void AddScrap(int amount)
    {
        gameData.totalScrap += amount;
        SaveGame();
    }

    public void AddSurvivor(Survivor newSurvivor)
    {
        gameData.sanctuarySurvivors.Add(newSurvivor);
        SaveGame();
    }

    public int GetUpgradeCost(int currentLevel)
    {
        return BASE_UPGRADE_COST + ((currentLevel - 1) * COST_INCREASE_PER_LEVEL);
    }

    public int GetShieldUpgradeCost()
    {
        if (gameData.firstAidKitLevel >= 2) return 999999;
        return SHIELD_BASE_COST * (gameData.firstAidKitLevel + 1);
    }

    public bool TryPurchaseDamageUpgrade()
    {
        int cost = GetUpgradeCost(gameData.damageUpgradeLevel);
        if (gameData.totalScrap >= cost)
        {
            gameData.totalScrap -= cost;
            gameData.damageUpgradeLevel++;
            SaveGame();
            return true;
        }
        return false;
    }

    public bool TryPurchaseStartingSurvivorsUpgrade()
    {
        int cost = GetUpgradeCost(gameData.startingSurvivorsUpgradeLevel);
        if (gameData.totalScrap >= cost)
        {
            gameData.totalScrap -= cost;
            gameData.startingSurvivorsUpgradeLevel++;
            SaveGame();
            return true;
        }
        return false;
    }

    public bool TryPurchaseScrapValueUpgrade()
    {
        int cost = GetUpgradeCost(gameData.scrapValueUpgradeLevel);
        if (gameData.totalScrap >= cost)
        {
            gameData.totalScrap -= cost;
            gameData.scrapValueUpgradeLevel++;
            SaveGame();
            return true;
        }
        return false;
    }

    public bool TryPurchaseFirstAidKitUpgrade()
    {
        if (gameData.firstAidKitLevel >= 2) return false;
        int cost = GetShieldUpgradeCost();
        if (gameData.totalScrap >= cost)
        {
            gameData.totalScrap -= cost;
            gameData.firstAidKitLevel++;
            SaveGame();
            return true;
        }
        return false;
    }
    #endregion
}