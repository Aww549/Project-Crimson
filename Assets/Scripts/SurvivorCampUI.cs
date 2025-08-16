// SurvivorCampUI.cs - Complete Rewrite by Jules
// Architecture: State Machine controlling CanvasGroups.
// Objective: Resolve UI rendering bugs by providing a stable, drop-in replacement.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SurvivorCampUI : MonoBehaviour
{
    /// <summary>
    /// Defines the primary UI states for the Survivor Camp screen.
    /// Each state corresponds to a distinct CanvasGroup-controlled panel.
    /// </summary>
    private enum UIPanel
    {
        Upgrades,
        Sanctuary_Main,
        Sanctuary_Details,
        Sanctuary_Selection
    }

    private UIPanel currentPanelState;

    [Header("--- UI Panel Canvas Groups ---")]
    [Tooltip("CanvasGroup for the main Upgrades panel.")]
    public CanvasGroup upgradesCG;
    [Tooltip("CanvasGroup for the main Sanctuary view (roster and missions).")]
    public CanvasGroup sanctuaryMainCG;
    [Tooltip("CanvasGroup for the Mission Details view.")]
    public CanvasGroup sanctuaryDetailsCG;
    [Tooltip("CanvasGroup for the Survivor Selection view for a mission.")]
    public CanvasGroup sanctuarySelectionCG;

    [Header("--- Scene Management ---")]
    public string mainGameSceneName = "SampleScene";
    public string mainMenuSceneName = "MainMenu";

    [Header("--- Upgrades UI ---")]
    public TextMeshProUGUI totalScrapText;
    public TextMeshProUGUI damageLevelText;
    public TextMeshProUGUI damageCostText;
    public TextMeshProUGUI startingSurvivorsLevelText;
    public TextMeshProUGUI startingSurvivorsCostText;
    public TextMeshProUGUI scrapValueLevelText;
    public TextMeshProUGUI scrapValueCostText;
    public TextMeshProUGUI firstAidKitLevelText;
    public TextMeshProUGUI firstAidKitCostText;

    [Header("--- Sanctuary: Roster & Missions ---")]
    public GameObject survivorListContent;
    public GameObject survivorListItemPrefab;
    public GameObject missionListContent;
    public GameObject missionListItemPrefab;

    [Header("--- Sanctuary: Details Panel ---")]
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDescriptionText;
    public TextMeshProUGUI missionRewardText;
    public TextMeshProUGUI missionDurationText;
    public TextMeshProUGUI missionSuccessChanceText;

    [Header("--- Sanctuary: Selection Panel ---")]
    public GameObject survivorSelectionContent;
    public GameObject survivorSelectionItemPrefab;
    public Button startMissionButton;

    // Private state variables
    private MissionData selectedMission;
    private List<Survivor> selectedSurvivorsForMission = new List<Survivor>();
    private List<CanvasGroup> allPanelCGs;

    #region Unity Lifecycle & State Machine

    void Awake()
    {
        // Populate the list of all canvas groups for easy management
        allPanelCGs = new List<CanvasGroup>
        {
            upgradesCG,
            sanctuaryMainCG,
            sanctuaryDetailsCG,
            sanctuarySelectionCG
        };
    }

    void Start()
    {
        // Set the initial state of the UI
        ChangeState(UIPanel.Upgrades);
        UpdateUpgradesUI();
    }

    /// <summary>
    /// The single, authoritative function for changing the visible UI panel.
    /// </summary>
    private void ChangeState(UIPanel newState)
    {
        currentPanelState = newState;

        // Disable all panels first
        foreach (var cg in allPanelCGs)
        {
            SetCanvasGroupState(cg, false);
        }

        // Enable the target panel
        switch (newState)
        {
            case UIPanel.Upgrades:
                SetCanvasGroupState(upgradesCG, true);
                break;
            case UIPanel.Sanctuary_Main:
                SetCanvasGroupState(sanctuaryMainCG, true);
                break;
            case UIPanel.Sanctuary_Details:
                SetCanvasGroupState(sanctuaryDetailsCG, true);
                break;
            case UIPanel.Sanctuary_Selection:
                SetCanvasGroupState(sanctuarySelectionCG, true);
                break;
        }
    }

    /// <summary>
    /// Controls the visibility and interactivity of a UI panel using its CanvasGroup.
    /// This is the ONLY method that should be used to show/hide panels.
    /// </summary>
    private void SetCanvasGroupState(CanvasGroup cg, bool active)
    {
        if (cg == null)
        {
            Debug.LogWarning($"A CanvasGroup is not assigned in the SurvivorCampUI inspector.", this);
            return;
        }
        cg.alpha = active ? 1f : 0f;
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }

    #endregion

    #region Public UI Methods (Button Hooks)

    // --- Main Panel Toggling ---

    public void ShowUpgradesPanel()
    {
        ChangeState(UIPanel.Upgrades);
        UpdateUpgradesUI();
    }

    public void ShowSanctuaryPanel()
    {
        ChangeState(UIPanel.Sanctuary_Main);
        RefreshSanctuaryUI();
    }

    // --- Sanctuary State Changes ---

    public void ShowMissionDetails(MissionData missionData)
    {
        selectedMission = missionData;
        selectedSurvivorsForMission.Clear(); // Clear previous selections

        // Populate Details Panel UI
        missionNameText.text = missionData.missionName;
        missionDescriptionText.text = missionData.description;
        missionRewardText.text = $"Base Reward: {missionData.baseRewardAmount} {missionData.rewardType}";
        missionDurationText.text = $"Duration: {missionData.durationHours} Hours";

        UpdateSuccessChanceDisplay();
        ChangeState(UIPanel.Sanctuary_Details);
    }

    public void OpenSurvivorSelection()
    {
        if (selectedMission == null) return;
        ChangeState(UIPanel.Sanctuary_Selection);
        PopulateSurvivorSelectionList();
    }

    public void ReturnToMainSanctuaryView()
    {
        selectedMission = null;
        ChangeState(UIPanel.Sanctuary_Main);
    }

    public void BackFromSelectionToDetails()
    {
        ChangeState(UIPanel.Sanctuary_Details);
    }

    public void OnStartMissionClicked()
    {
        if (selectedMission != null && selectedSurvivorsForMission.Count > 0)
        {
            MissionController.Instance.StartMission(selectedMission, selectedSurvivorsForMission);
            selectedMission = null;
            ChangeState(UIPanel.Sanctuary_Main);
            RefreshSanctuaryUI();
        }
    }

    // --- Scene Navigation ---

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnStartRunClicked()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }

    // --- Upgrade Purchases ---

    public void OnPurchaseDamageClicked()
    {
        if (GameDataManager.Instance.TryPurchaseDamageUpgrade()) UpdateUpgradesUI();
    }

    public void OnPurchaseFirstAidKitClicked()
    {
        if (GameDataManager.Instance.TryPurchaseFirstAidKitUpgrade()) UpdateUpgradesUI();
    }

    public void OnPurchaseScrapValueClicked()
    {
        if (GameDataManager.Instance.TryPurchaseScrapValueUpgrade()) UpdateUpgradesUI();
    }

    public void OnPurchaseStartingSurvivorsClicked()
    {
        if (GameDataManager.Instance.TryPurchaseStartingSurvivorsUpgrade()) UpdateUpgradesUI();
    }

    #endregion

    #region UI Population & Data Handling

    public void UpdateUpgradesUI()
    {
        if (GameDataManager.Instance == null) return;
        totalScrapText.text = "SCRAP: " + GameDataManager.Instance.gameData.totalScrap;

        int damageLevel = GameDataManager.Instance.gameData.damageUpgradeLevel;
        damageLevelText.text = "LVL " + damageLevel;
        damageCostText.text = GameDataManager.Instance.GetUpgradeCost(damageLevel) + " SCRAP";

        int startingSurvivorsLevel = GameDataManager.Instance.gameData.startingSurvivorsUpgradeLevel;
        startingSurvivorsLevelText.text = "LVL " + startingSurvivorsLevel;
        startingSurvivorsCostText.text = GameDataManager.Instance.GetUpgradeCost(startingSurvivorsLevel) + " SCRAP";

        int scrapValueLevel = GameDataManager.Instance.gameData.scrapValueUpgradeLevel;
        scrapValueLevelText.text = "LVL " + scrapValueLevel;
        scrapValueCostText.text = GameDataManager.Instance.GetUpgradeCost(scrapValueLevel) + " SCRAP";

        int shieldLevel = GameDataManager.Instance.gameData.firstAidKitLevel;
        firstAidKitLevelText.text = "LVL " + shieldLevel;
        if (shieldLevel >= 2) { firstAidKitCostText.text = "MAX"; }
        else { firstAidKitCostText.text = GameDataManager.Instance.GetShieldUpgradeCost() + " SCRAP"; }
    }

    public void RefreshSanctuaryUI()
    {
        PopulateSurvivorList();
        PopulateMissionList();
    }

    private void PopulateSurvivorList()
    {
        foreach (Transform child in survivorListContent.transform) { Destroy(child.gameObject); }
        if (GameDataManager.Instance == null || GameDataManager.Instance.gameData.sanctuarySurvivors.Count == 0) return;
        foreach (var survivor in GameDataManager.Instance.gameData.sanctuarySurvivors)
        {
            GameObject newItem = Instantiate(survivorListItemPrefab, survivorListContent.transform);
            newItem.GetComponent<SurvivorListItemUI>()?.Setup(survivor);
        }
    }

    private void PopulateMissionList()
    {
        foreach (Transform child in missionListContent.transform) { Destroy(child.gameObject); }
        var allMissions = Resources.LoadAll<MissionData>("Missions").ToList();
        if (allMissions.Count == 0) return;
        foreach (var missionData in allMissions)
        {
            GameObject newItem = Instantiate(missionListItemPrefab, missionListContent.transform);
            newItem.GetComponent<MissionListItemUI>()?.Setup(missionData, this);
        }
    }

    private void PopulateSurvivorSelectionList()
    {
        foreach (Transform child in survivorSelectionContent.transform) { Destroy(child.gameObject); }
        var idleSurvivors = GameDataManager.Instance.gameData.sanctuarySurvivors
            .Where(s => s.status == SurvivorStatus.Idle).ToList();
        foreach (var survivor in idleSurvivors)
        {
            GameObject newItem = Instantiate(survivorSelectionItemPrefab, survivorSelectionContent.transform);
            newItem.GetComponent<SurvivorSelectionItemUI>()?.Setup(survivor, this);
        }
    }

    public void OnSurvivorToggleChanged(Survivor survivor, bool isSelected)
    {
        if (isSelected)
        {
            if (!selectedSurvivorsForMission.Contains(survivor) && selectedSurvivorsForMission.Count < selectedMission.maxSurvivors)
            {
                selectedSurvivorsForMission.Add(survivor);
            }
        }
        else
        {
            selectedSurvivorsForMission.Remove(survivor);
        }
        UpdateSuccessChanceDisplay();
    }

    private void UpdateSuccessChanceDisplay()
    {
        if (selectedMission == null) return;

        float finalChance = selectedMission.baseSuccessChance;
        if (selectedSurvivorsForMission.Count > 1)
        {
            finalChance += (selectedSurvivorsForMission.Count - 1) * selectedMission.bonusSuccessChancePerSurvivor;
        }
        foreach (var survivor in selectedSurvivorsForMission)
        {
            foreach (var trait in survivor.traits)
            {
                finalChance += trait.successChanceModifier;
            }
        }
        finalChance = Mathf.Clamp01(finalChance);

        missionSuccessChanceText.text = $"Success Chance: <color=yellow>{(finalChance * 100):F0}%</color>";
        startMissionButton.interactable = selectedSurvivorsForMission.Count > 0;
    }

    #endregion
}
