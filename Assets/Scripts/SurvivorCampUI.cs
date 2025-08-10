using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages the UI for the Survivor Camp scene.
/// Displays player data and handles button clicks for purchasing upgrades and starting a new run.
/// </summary>
public class SurvivorCampUI : MonoBehaviour
{
    [Header("Text Elements")]
    public TextMeshProUGUI totalScrapText;
    public TextMeshProUGUI damageLevelText;
    public TextMeshProUGUI damageCostText;
    // *** RENAME: Changed to reflect the new Starting Survivors upgrade ***
    public TextMeshProUGUI startingSurvivorsLevelText;
    public TextMeshProUGUI startingSurvivorsCostText;
    public TextMeshProUGUI scrapValueLevelText;
    public TextMeshProUGUI scrapValueCostText;

    [Header("Scene Management")]
    public string mainGameSceneName = "SampleScene"; // The name of your main gameplay scene

    void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("SurvivorCampUI Error: GameDataManager.Instance is not found!");
            return;
        }

        totalScrapText.text = "SCRAP: " + GameDataManager.Instance.TotalScrap;

        // Update Damage Upgrade display
        int damageLevel = GameDataManager.Instance.DamageUpgradeLevel;
        damageLevelText.text = "LVL " + damageLevel;
        damageCostText.text = GameDataManager.Instance.GetUpgradeCost(damageLevel) + " SCRAP";

        // *** FIX: Update Starting Survivors (formerly Fire Rate) Upgrade display ***
        int startingSurvivorsLevel = GameDataManager.Instance.StartingSurvivorsUpgradeLevel;
        startingSurvivorsLevelText.text = "LVL " + startingSurvivorsLevel;
        startingSurvivorsCostText.text = GameDataManager.Instance.GetUpgradeCost(startingSurvivorsLevel) + " SCRAP";

        // Update Scrap Value Upgrade display
        int scrapValueLevel = GameDataManager.Instance.ScrapValueUpgradeLevel;
        scrapValueLevelText.text = "LVL " + scrapValueLevel;
        scrapValueCostText.text = GameDataManager.Instance.GetUpgradeCost(scrapValueLevel) + " SCRAP";
    }

    public void OnPurchaseDamageClicked()
    {
        bool success = GameDataManager.Instance.TryPurchaseDamageUpgrade();
        if (success)
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// *** FIX: This function now correctly calls the Starting Survivors upgrade purchase method. ***
    /// </summary>
    public void OnPurchaseStartingSurvivorsClicked()
    {
        bool success = GameDataManager.Instance.TryPurchaseStartingSurvivorsUpgrade();
        if (success)
        {
            UpdateUI();
        }
    }

    public void OnPurchaseScrapValueClicked()
    {
        bool success = GameDataManager.Instance.TryPurchaseScrapValueUpgrade();
        if (success)
        {
            UpdateUI();
        }
    }

    public void OnStartRunClicked()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }
}
