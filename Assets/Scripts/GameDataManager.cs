using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    // --- Player Data ---
    public int TotalScrap { get; private set; }
    public int DamageUpgradeLevel { get; private set; }
    public int StartingSurvivorsUpgradeLevel { get; private set; }
    public int ScrapValueUpgradeLevel { get; private set; }
    // *** NEW: The level of the First Aid Kit upgrade. Level 0 = no shield. ***
    public int FirstAidKitLevel { get; private set; }

    // --- Upgrade Costs ---
    private const int BASE_UPGRADE_COST = 50;
    private const int COST_INCREASE_PER_LEVEL = 25;
    // *** NEW: A higher base cost for the powerful shield upgrade. ***
    private const int SHIELD_BASE_COST = 250;

    void Awake()
    {
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
        StartingSurvivorsUpgradeLevel = PlayerPrefs.GetInt("StartingSurvivorsUpgradeLevel", 1);
        ScrapValueUpgradeLevel = PlayerPrefs.GetInt("ScrapValueUpgradeLevel", 1);
        // *** NEW: Load the shield level, defaulting to 0 (none). ***
        FirstAidKitLevel = PlayerPrefs.GetInt("FirstAidKitLevel", 0);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("TotalScrap", TotalScrap);
        PlayerPrefs.SetInt("DamageUpgradeLevel", DamageUpgradeLevel);
        PlayerPrefs.SetInt("StartingSurvivorsUpgradeLevel", StartingSurvivorsUpgradeLevel);
        PlayerPrefs.SetInt("ScrapValueUpgradeLevel", ScrapValueUpgradeLevel);
        // *** NEW: Save the shield level. ***
        PlayerPrefs.SetInt("FirstAidKitLevel", FirstAidKitLevel);
        PlayerPrefs.Save();
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

    // *** NEW: A separate cost calculation for the shield. ***
    public int GetShieldUpgradeCost()
    {
        // Level 0 cost = 250. Level 1 cost = 500. Max level is 2.
        if (FirstAidKitLevel >= 2) return 999999; // Effectively infinite cost if maxed out
        return SHIELD_BASE_COST * (FirstAidKitLevel + 1);
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

    /// <summary>
    /// *** NEW: Attempts to purchase the next First Aid Kit upgrade. ***
    /// </summary>
    public bool TryPurchaseFirstAidKitUpgrade()
    {
        if (FirstAidKitLevel >= 2) return false; // Can't upgrade past level 2

        int cost = GetShieldUpgradeCost();
        if (TotalScrap >= cost)
        {
            TotalScrap -= cost;
            FirstAidKitLevel++;
            SaveData();
            return true;
        }
        return false;
    }
}
