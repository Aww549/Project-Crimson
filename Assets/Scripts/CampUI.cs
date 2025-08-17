using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CampUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;

    [Header("Main Panel Buttons")]
    [SerializeField] private Button missionsButton;
    [SerializeField] private Button exitToMainMenuButton;

    [Header("Building Upgrade Buttons")]
    [SerializeField] private Button armoryUpgradeButton;
    [SerializeField] private Button hospitalUpgradeButton;
    [SerializeField] private Button workshopUpgradeButton;
    [SerializeField] private Button townHallUpgradeButton;

    [Header("Currency Display")]
    [SerializeField] private TextMeshProUGUI scrapText;
    [SerializeField] private TextMeshProUGUI materialsText;

    [Header("Level Display")]
    [SerializeField] private TextMeshProUGUI armoryLevelText;
    [SerializeField] private TextMeshProUGUI hospitalLevelText;
    [SerializeField] private TextMeshProUGUI workshopLevelText;
    [SerializeField] private TextMeshProUGUI townHallLevelText;

    private void Start()
    {
        SetupButtonListeners();
        UpdateCurrencyDisplay();
        UpdateUpgradeLevelsDisplay();
        ShowMainPanel();
    }

    private void SetupButtonListeners()
    {
        // Main panel buttons
        missionsButton.onClick.AddListener(OnMissionsButtonClick);
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
    }

    private void UpdateUpgradeLevelsDisplay()
    {
        if (GameDataManager.Instance == null) return;

        armoryLevelText.text = $"Level {GameDataManager.Instance.gameData.armoryLevel}";
        hospitalLevelText.text = $"Level {GameDataManager.Instance.gameData.hospitalLevel}";
        workshopLevelText.text = $"Level {GameDataManager.Instance.gameData.workshopLevel}";
        // Assuming town hall doesn't have a level in GameData, or add it if it does
        // townHallLevelText.text = $"Level {GameDataManager.Instance.gameData.townHallLevel}";
    }

    private void OnBuildingUpgrade(string buildingName)
    {
        // Implementation for building upgrades
        if (GameDataManager.Instance != null)
        {
            // Add your upgrade logic here
            UpdateCurrencyDisplay();
            UpdateUpgradeLevelsDisplay();
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (GameDataManager.Instance != null)
        {
            scrapText.text = $"Scrap: {GameDataManager.Instance.gameData.totalScrap}";
            materialsText.text = $"Materials: {GameDataManager.Instance.gameData.materials}";
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
        if (exitToMainMenuButton != null) exitToMainMenuButton.onClick.RemoveAllListeners();
        if (armoryUpgradeButton != null) armoryUpgradeButton.onClick.RemoveAllListeners();
        if (hospitalUpgradeButton != null) hospitalUpgradeButton.onClick.RemoveAllListeners();
        if (workshopUpgradeButton != null) workshopUpgradeButton.onClick.RemoveAllListeners();
        if (townHallUpgradeButton != null) townHallUpgradeButton.onClick.RemoveAllListeners();
    }
}