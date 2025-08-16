using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject upgradesPanel;

    [Header("Main Panel Buttons")]
    [SerializeField] private Button missionsButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button exitToMainMenuButton;

    [Header("Building Upgrade Buttons")]
    [SerializeField] private Button armoryUpgradeButton;
    [SerializeField] private Button hospitalUpgradeButton;
    [SerializeField] private Button workshopUpgradeButton;
    [SerializeField] private Button townHallUpgradeButton;

    [Header("Currency Display")]
    [SerializeField] private Text scrapText;
    [SerializeField] private Text materialsText;

    private void Start()
    {
        SetupButtonListeners();
        UpdateCurrencyDisplay();
        ShowMainPanel();
    }

    private void SetupButtonListeners()
    {
        // Main panel buttons
        missionsButton.onClick.AddListener(OnMissionsButtonClick);
        upgradesButton.onClick.AddListener(ShowUpgradesPanel);
        exitToMainMenuButton.onClick.AddListener(OnExitToMainMenu);

        // Building upgrade buttons
        armoryUpgradeButton.onClick.AddListener(() => OnBuildingUpgrade("Armory"));
        hospitalUpgradeButton.onClick.AddListener(() => OnBuildingUpgrade("Hospital"));
        workshopUpgradeButton.onClick.AddListener(() => OnBuildingUpgrade("Workshop"));
        townHallUpgradeButton.onClick.AddListener(() => OnBuildingUpgrade("TownHall"));
    }

    private void OnMissionsButtonClick()
    {
        SceneTransitionManager.Instance.LoadMissionsScene();
    }

    private void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        upgradesPanel.SetActive(false);
    }

    private void ShowUpgradesPanel()
    {
        mainPanel.SetActive(false);
        upgradesPanel.SetActive(true);
    }

    private void OnBuildingUpgrade(string buildingName)
    {
        // Implementation for building upgrades
        if (GameDataManager.Instance != null)
        {
            // Add your upgrade logic here
            UpdateCurrencyDisplay();
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (GameDataManager.Instance != null)
        {
            scrapText.text = $"Scrap: {GameDataManager.Instance.gameData.scrapCurrency}";
            materialsText.text = $"Materials: {GameDataManager.Instance.gameData.materialsCurrency}";
        }
    }

    private void OnExitToMainMenu()
    {
        // Implement your main menu loading logic here
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (missionsButton != null) missionsButton.onClick.RemoveAllListeners();
        if (upgradesButton != null) upgradesButton.onClick.RemoveAllListeners();
        if (exitToMainMenuButton != null) exitToMainMenuButton.onClick.RemoveAllListeners();
        if (armoryUpgradeButton != null) armoryUpgradeButton.onClick.RemoveAllListeners();
        if (hospitalUpgradeButton != null) hospitalUpgradeButton.onClick.RemoveAllListeners();
        if (workshopUpgradeButton != null) workshopUpgradeButton.onClick.RemoveAllListeners();
        if (townHallUpgradeButton != null) townHallUpgradeButton.onClick.RemoveAllListeners();
    }
}