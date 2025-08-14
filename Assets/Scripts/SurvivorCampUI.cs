using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SurvivorCampUI : MonoBehaviour
{
    [Header("Text Elements")]
    public TextMeshProUGUI totalScrapText;
    public TextMeshProUGUI damageLevelText;
    public TextMeshProUGUI damageCostText;
    public TextMeshProUGUI startingSurvivorsLevelText;
    public TextMeshProUGUI startingSurvivorsCostText;
    public TextMeshProUGUI scrapValueLevelText;
    public TextMeshProUGUI scrapValueCostText;
    public TextMeshProUGUI firstAidKitLevelText;
    public TextMeshProUGUI firstAidKitCostText;

    [Header("Scene Management")]
    public string mainGameSceneName = "SampleScene";
    // *** NEW: A reference to the Main Menu scene name ***
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameDataManager.Instance == null) return;

        totalScrapText.text = "SCRAP: " + GameDataManager.Instance.TotalScrap;

        // Damage Upgrade
        int damageLevel = GameDataManager.Instance.DamageUpgradeLevel;
        damageLevelText.text = "LVL " + damageLevel;
        damageCostText.text = GameDataManager.Instance.GetUpgradeCost(damageLevel) + " SCRAP";

        // Starting Survivors Upgrade
        int startingSurvivorsLevel = GameDataManager.Instance.StartingSurvivorsUpgradeLevel;
        startingSurvivorsLevelText.text = "LVL " + startingSurvivorsLevel;
        startingSurvivorsCostText.text = GameDataManager.Instance.GetUpgradeCost(startingSurvivorsLevel) + " SCRAP";

        // Scrap Value Upgrade
        int scrapValueLevel = GameDataManager.Instance.ScrapValueUpgradeLevel;
        scrapValueLevelText.text = "LVL " + scrapValueLevel;
        scrapValueCostText.text = GameDataManager.Instance.GetUpgradeCost(scrapValueLevel) + " SCRAP";

        // First Aid Kit
        int shieldLevel = GameDataManager.Instance.FirstAidKitLevel;
        firstAidKitLevelText.text = "LVL " + shieldLevel;
        if (shieldLevel >= 2)
        {
            firstAidKitCostText.text = "MAX";
        }
        else
        {
            firstAidKitCostText.text = GameDataManager.Instance.GetShieldUpgradeCost() + " SCRAP";
        }
    }

    /// <summary>
    /// *** NEW: This function should be linked to the "Back" button. ***
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnPurchaseFirstAidKitClicked()
    {
        if (GameDataManager.Instance.TryPurchaseFirstAidKitUpgrade()) UpdateUI();
    }

    public void OnPurchaseDamageClicked()
    {
        if (GameDataManager.Instance.TryPurchaseDamageUpgrade()) UpdateUI();
    }

    public void OnPurchaseStartingSurvivorsClicked()
    {
        if (GameDataManager.Instance.TryPurchaseStartingSurvivorsUpgrade()) UpdateUI();
    }

    public void OnPurchaseScrapValueClicked()
    {
        if (GameDataManager.Instance.TryPurchaseScrapValueUpgrade()) UpdateUI();
    }

    public void OnStartRunClicked()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }
}
