using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CampUI : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField] private Button missionsButton;
    [SerializeField] private Button exitToMainMenuButton;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button combatTrainingButton; // Corresponds to damageUpgradeLevel
    [SerializeField] private Button firstAidKitButton;    // Corresponds to firstAidKitLevel
    [SerializeField] private Button scrapScavengingButton; // Corresponds to scrapValueUpgradeLevel
    [SerializeField] private Button fortifyAmmoButton;     // Corresponds to startingSurvivorsUpgradeLevel

    [Header("Currency Display")]
    [SerializeField] private TextMeshProUGUI scrapText;
    [SerializeField] private TextMeshProUGUI materialsText;

    [Header("Level Display")]
    [SerializeField] private TextMeshProUGUI combatTrainingLevelText;
    [SerializeField] private TextMeshProUGUI firstAidKitLevelText;
    [SerializeField] private TextMeshProUGUI scrapScavengingLevelText;
    [SerializeField] private TextMeshProUGUI fortifyAmmoLevelText;

    [Header("Cost Display")]
    [SerializeField] private TextMeshProUGUI combatTrainingCostText;
    [SerializeField] private TextMeshProUGUI firstAidKitCostText;
    [SerializeField] private TextMeshProUGUI scrapScavengingCostText;
    [SerializeField] private TextMeshProUGUI fortifyAmmoCostText;

    private void Start()
    {
        SetupButtonListeners();
        UpdateAllDisplays();
    }

    private void SetupButtonListeners()
    {
        // Navigation
        if (missionsButton != null) missionsButton.onClick.AddListener(OnMissionsButtonClick);
        if (exitToMainMenuButton != null) exitToMainMenuButton.onClick.AddListener(OnExitToMainMenu);

        // Upgrades
        if (combatTrainingButton != null) combatTrainingButton.onClick.AddListener(OnPurchaseCombatTraining);
        if (firstAidKitButton != null) firstAidKitButton.onClick.AddListener(OnPurchaseFirstAidKit);
        if (scrapScavengingButton != null) scrapScavengingButton.onClick.AddListener(OnPurchaseScrapScavenging);
        if (fortifyAmmoButton != null) fortifyAmmoButton.onClick.AddListener(OnPurchaseFortifyAmmo);
    }

    private void UpdateAllDisplays()
    {
        UpdateCurrencyDisplay();
        UpdateUpgradeLevelsDisplay();
        UpdateUpgradeCostsDisplay();
    }

    private void UpdateUpgradeCostsDisplay()
    {
        if (GameDataManager.Instance == null) return;

        combatTrainingCostText.text = $"Cost: {GameDataManager.Instance.GetUpgradeCost(GameDataManager.Instance.gameData.damageUpgradeLevel)}";
        firstAidKitCostText.text = $"Cost: {GameDataManager.Instance.GetShieldUpgradeCost()}";
        scrapScavengingCostText.text = $"Cost: {GameDataManager.Instance.GetUpgradeCost(GameDataManager.Instance.gameData.scrapValueUpgradeLevel)}";
        fortifyAmmoCostText.text = $"Cost: {GameDataManager.Instance.GetUpgradeCost(GameDataManager.Instance.gameData.startingSurvivorsUpgradeLevel)}";
    }

    private void UpdateUpgradeLevelsDisplay()
    {
        if (GameDataManager.Instance == null) return;

        combatTrainingLevelText.text = $"Level {GameDataManager.Instance.gameData.damageUpgradeLevel}";
        firstAidKitLevelText.text = $"Level {GameDataManager.Instance.gameData.firstAidKitLevel}";
        scrapScavengingLevelText.text = $"Level {GameDataManager.Instance.gameData.scrapValueUpgradeLevel}";
        fortifyAmmoLevelText.text = $"Level {GameDataManager.Instance.gameData.startingSurvivorsUpgradeLevel}";
    }

    private void UpdateCurrencyDisplay()
    {
        if (GameDataManager.Instance != null)
        {
            scrapText.text = $"Scrap: {GameDataManager.Instance.gameData.totalScrap}";
            materialsText.text = $"Materials: {GameDataManager.Instance.gameData.materials}";
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
        if (GameDataManager.Instance != null)
        {
            if (GameDataManager.Instance.TryPurchaseDamageUpgrade())
            {
                Debug.Log("Purchased Combat Training Upgrade!");
                UpdateAllDisplays();
            }
            else
            {
                Debug.Log("Not enough scrap for Combat Training Upgrade.");
            }
        }
    }

    public void OnPurchaseFirstAidKit()
    {
        if (GameDataManager.Instance != null)
        {
            if (GameDataManager.Instance.TryPurchaseFirstAidKitUpgrade())
            {
                Debug.Log("Purchased First Aid Kit Upgrade!");
                UpdateAllDisplays();
            }
            else
            {
                Debug.Log("Not enough scrap for First Aid Kit Upgrade.");
            }
        }
    }

    public void OnPurchaseScrapScavenging()
    {
        if (GameDataManager.Instance != null)
        {
            if (GameDataManager.Instance.TryPurchaseScrapValueUpgrade())
            {
                Debug.Log("Purchased Scrap Scavenging Upgrade!");
                UpdateAllDisplays();
            }
            else
            {
                Debug.Log("Not enough scrap for Scrap Scavenging Upgrade.");
            }
        }
    }

    public void OnPurchaseFortifyAmmo()
    {
        if (GameDataManager.Instance != null)
        {
            if (GameDataManager.Instance.TryPurchaseStartingSurvivorsUpgrade())
            {
                Debug.Log("Purchased Fortify Ammo (Starting Survivors) Upgrade!");
                UpdateAllDisplays();
            }
            else
            {
                Debug.Log("Not enough scrap for Fortify Ammo (Starting Survivors) Upgrade.");
            }
        }
    }


    private void OnDestroy()
    {
        // Clean up listeners
        if (missionsButton != null) missionsButton.onClick.RemoveAllListeners();
        if (exitToMainMenuButton != null) exitToMainMenuButton.onClick.RemoveAllListeners();
        if (combatTrainingButton != null) combatTrainingButton.onClick.RemoveAllListeners();
        if (firstAidKitButton != null) firstAidKitButton.onClick.RemoveAllListeners();
        if (scrapScavengingButton != null) scrapScavengingButton.onClick.RemoveAllListeners();
        if (fortifyAmmoButton != null) fortifyAmmoButton.onClick.RemoveAllListeners();
    }
}