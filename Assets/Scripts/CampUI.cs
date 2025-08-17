using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// This script has been rewritten to match the scene hierarchy provided by the user.
// The variable names for buttons and text fields should now directly correspond
// to the names of the GameObjects in the scene, making them easy to assign.
public class CampUI : MonoBehaviour
{
    // === HIERARCHY: Main Panel ===
    [Header("Main Panel")]
    [SerializeField] private Button MissionsButton;
    [SerializeField] private Button BackButton; // Assuming this is the exit button
    [SerializeField] private TextMeshProUGUI TotalScrap_Text;

    // === HIERARCHY: Upgrades Panel -> DamageUpgrade_Row ===
    [Header("Damage Upgrade")]
    [SerializeField] private TextMeshProUGUI DamageLevel_Text;
    [SerializeField] private Button BuyDamage_Button;
    [SerializeField] private TextMeshProUGUI DamageCost_Text;

    // === HIERARCHY: Upgrades Panel -> StartingSurvivorsUpgrade_Row ===
    [Header("Starting Survivors Upgrade")]
    [SerializeField] private TextMeshProUGUI StartingSurvivorsLevel_Text;
    [SerializeField] private Button BuyStartingSurvivors_Button;
    [SerializeField] private TextMeshProUGUI StartingSurvivorsCost_Text;

    // === HIERARCHY: Upgrades Panel -> ScrapValueUpgrade_Row ===
    [Header("Scrap Value Upgrade")]
    [SerializeField] private TextMeshProUGUI ScrapValueLevel_Text;
    [SerializeField] private Button BuyScrapValue_Button;
    [SerializeField] private TextMeshProUGUI ScrapValueCost_Text;

    // === HIERARCHY: Upgrades Panel -> FirstAidKit_Row ===
    [Header("First Aid Kit Upgrade")]
    [SerializeField] private TextMeshProUGUI FirstAidKitLevel_Text;
    [SerializeField] private Button BuyFirstAidKit_Button;
    [SerializeField] private TextMeshProUGUI FirstAidKitCost_Text;


    void Start()
    {
        SetupButtonListeners();
        UpdateAllDisplays();
    }

    private void SetupButtonListeners()
    {
        // Navigation
        if (MissionsButton != null) MissionsButton.onClick.AddListener(OnMissionsButtonClick);
        if (BackButton != null) BackButton.onClick.AddListener(OnExitToMainMenu);

        // Upgrades
        if (BuyDamage_Button != null) BuyDamage_Button.onClick.AddListener(OnPurchaseCombatTraining);
        if (BuyFirstAidKit_Button != null) BuyFirstAidKit_Button.onClick.AddListener(OnPurchaseFirstAidKit);
        if (BuyScrapValue_Button != null) BuyScrapValue_Button.onClick.AddListener(OnPurchaseScrapScavenging);
        if (BuyStartingSurvivors_Button != null) BuyStartingSurvivors_Button.onClick.AddListener(OnPurchaseStartingSurvivors);
    }

    private void UpdateAllDisplays()
    {
        UpdateCurrencyDisplay();
        UpdateUpgradeLevelsAndCosts();
    }

    private void UpdateUpgradeLevelsAndCosts()
    {
        if (GameDataManager.Instance == null) return;

        // Damage / Combat Training
        if (DamageLevel_Text != null) DamageLevel_Text.text = $"Level {GameDataManager.Instance.gameData.damageUpgradeLevel}";
        if (DamageCost_Text != null) DamageCost_Text.text = $"Cost: {GameDataManager.Instance.GetUpgradeCost(GameDataManager.Instance.gameData.damageUpgradeLevel)}";

        // First Aid Kit
        if (FirstAidKitLevel_Text != null) FirstAidKitLevel_Text.text = $"Level {GameDataManager.Instance.gameData.firstAidKitLevel}";
        if (FirstAidKitCost_Text != null) FirstAidKitCost_Text.text = $"Cost: {GameDataManager.Instance.GetShieldUpgradeCost()}";

        // Scrap Scavenging
        if (ScrapValueLevel_Text != null) ScrapValueLevel_Text.text = $"Level {GameDataManager.Instance.gameData.scrapValueUpgradeLevel}";
        if (ScrapValueCost_Text != null) ScrapValueCost_Text.text = $"Cost: {GameDataManager.Instance.GetUpgradeCost(GameDataManager.Instance.gameData.scrapValueUpgradeLevel)}";

        // Starting Survivors / Fortify Ammo
        if (StartingSurvivorsLevel_Text != null) StartingSurvivorsLevel_Text.text = $"Level {GameDataManager.Instance.gameData.startingSurvivorsUpgradeLevel}";
        if (StartingSurvivorsCost_Text != null) StartingSurvivorsCost_Text.text = $"Cost: {GameDataManager.Instance.GetUpgradeCost(GameDataManager.Instance.gameData.startingSurvivorsUpgradeLevel)}";
    }

    private void UpdateCurrencyDisplay()
    {
        if (GameDataManager.Instance != null && TotalScrap_Text != null)
        {
            TotalScrap_Text.text = $"Scrap: {GameDataManager.Instance.gameData.totalScrap}";
        }
    }

    // --- Public Button Handlers ---

    public void OnMissionsButtonClick()
    {
        SceneTransitionManager.Instance.LoadMissionsScene();
    }

    public void OnExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPurchaseCombatTraining()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.TryPurchaseDamageUpgrade())
        {
            UpdateAllDisplays();
        }
    }

    public void OnPurchaseFirstAidKit()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.TryPurchaseFirstAidKitUpgrade())
        {
            UpdateAllDisplays();
        }
    }

    public void OnPurchaseScrapScavenging()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.TryPurchaseScrapValueUpgrade())
        {
            UpdateAllDisplays();
        }
    }

    public void OnPurchaseStartingSurvivors()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.TryPurchaseStartingSurvivorsUpgrade())
        {
            UpdateAllDisplays();
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (MissionsButton != null) MissionsButton.onClick.RemoveAllListeners();
        if (BackButton != null) BackButton.onClick.RemoveAllListeners();
        if (BuyDamage_Button != null) BuyDamage_Button.onClick.RemoveAllListeners();
        if (BuyFirstAidKit_Button != null) BuyFirstAidKit_Button.onClick.RemoveAllListeners();
        if (BuyScrapValue_Button != null) BuyScrapValue_Button.onClick.RemoveAllListeners();
        if (BuyStartingSurvivors_Button != null) BuyStartingSurvivors_Button.onClick.RemoveAllListeners();
    }
}