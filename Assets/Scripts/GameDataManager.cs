using UnityEngine;

/// <summary>
/// A persistent singleton that manages the player's currency and permanent upgrades.
/// It handles saving and loading all persistent data for the meta-game.
/// </summary>
public class GameDataManager : MonoBehaviour
{
    // --- Singleton Instance ---
    public static GameDataManager Instance { get; private set; }

    // --- Player Data ---
    public int TotalScrap { get; private set; }
    public int DamageUpgradeLevel { get; private set; }
    // *** RENAME: This now represents the starting survivor count upgrade ***
    public int StartingSurvivorsUpgradeLevel { get; private set; }
    public int ScrapValueUpgradeLevel { get; private set; }

    // --- Upgrade Costs ---
    private const int BASE_UPGRADE_COST = 50;
    private const int COST_INCREASE_PER_LEVEL = 25;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadData()
    {
        TotalScrap = PlayerPrefs.GetInt("TotalScrap", 0);
        DamageUpgradeLevel = PlayerPrefs.GetInt("DamageUpgradeLevel", 1);
        // *** RENAME: Updated PlayerPrefs key for clarity ***
        StartingSurvivorsUpgradeLevel = PlayerPrefs.GetInt("StartingSurvivorsUpgradeLevel", 1);
        ScrapValueUpgradeLevel = PlayerPrefs.GetInt("ScrapValueUpgradeLevel", 1);
        Debug.Log("Game Data Loaded.");
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("TotalScrap", TotalScrap);
        PlayerPrefs.SetInt("DamageUpgradeLevel", DamageUpgradeLevel);
        // *** RENAME: Updated PlayerPrefs key for clarity ***
        PlayerPrefs.SetInt("StartingSurvivorsUpgradeLevel", StartingSurvivorsUpgradeLevel);
        PlayerPrefs.SetInt("ScrapValueUpgradeLevel", ScrapValueUpgradeLevel);
        PlayerPrefs.Save();
        Debug.Log("Game Data Saved.");
    }

    public void AddScrap(int amount)
    {
        TotalScrap += amount;
        SaveData();
    }

    public int GetUpgradeCost(int currentLevel)
    {
        return BASE_UPGRADE_COST + ((currentLevel - 1) * COST_INCREASE_PER_LEVEL);
    }

    public bool TryPurchaseDamageUpgrade()
    {
        int cost = GetUpgradeCost(DamageUpgradeLevel);
        if (TotalScrap >= cost)
        {
            TotalScrap -= cost;
            DamageUpgradeLevel++;
            SaveData();
            return true;
        }
        return false;
    }

    /// <summary>
    /// *** RENAME: This function now handles purchasing the Starting Survivors upgrade. ***
    /// </summary>
    public bool TryPurchaseStartingSurvivorsUpgrade()
    {
        int cost = GetUpgradeCost(StartingSurvivorsUpgradeLevel);
        if (TotalScrap >= cost)
        {
            TotalScrap -= cost;
            StartingSurvivorsUpgradeLevel++;
            SaveData();
            return true;
        }
        return false;
    }

    public bool TryPurchaseScrapValueUpgrade()
    {
        int cost = GetUpgradeCost(ScrapValueUpgradeLevel);
        if (TotalScrap >= cost)
        {
            TotalScrap -= cost;
            ScrapValueUpgradeLevel++;
            SaveData();
            return true;
        }
        return false;
    }
}
