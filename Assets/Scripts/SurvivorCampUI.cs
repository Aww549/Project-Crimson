// SurvivorCampUI.cs - Definitive Final Fix
// Architecture: A clean State Machine controlling CanvasGroups.
// Objective: Resolve all UI rendering and logical bugs.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SurvivorCampUI : MonoBehaviour
{
    // Defines the only possible states for the UI.
    private enum UIPanel { Upgrades, Sanctuary_Main, Sanctuary_Details, Sanctuary_Selection }

    [Header("--- UI Panel Canvas Groups ---")]
    [Tooltip("Drag the CanvasGroup of the UpgradesPanel here.")]
    public CanvasGroup upgradesCG;
    [Tooltip("Drag the CanvasGroup of the MainView panel here.")]
    public CanvasGroup sanctuaryMainCG;
    [Tooltip("Drag the CanvasGroup of the MissionDetailsPanel here.")]
    public CanvasGroup sanctuaryDetailsCG;
    [Tooltip("Drag the CanvasGroup of the SurvivorSelectionPanel here.")]
    public CanvasGroup sanctuarySelectionCG;

    [Header("--- Scene Management ---")]
    public string mainGameSceneName = "SampleScene";
    public string mainMenuSceneName = "MainMenu";

    #region Public UI References
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
    #endregion

    // Private state variables
    private MissionData selectedMission;
    private List<Survivor> selectedSurvivorsForMission = new List<Survivor>();
    private List<CanvasGroup> allPanelCGs;

    void Awake()
    {
        // Create a list of all CanvasGroups for easy management.
        allPanelCGs = new List<CanvasGroup> { upgradesCG, sanctuaryMainCG, sanctuaryDetailsCG, sanctuarySelectionCG };

        // DEFINITIVE FIX: Ensure all panels are disabled on awake to prevent user interaction
        // before the GameDataManager is ready and the UI has been initialized.
        foreach (var cg in allPanelCGs)
        {
            if (cg != null) SetCanvasGroupState(cg, false);
        }
    }

    // DEFINITIVE FIX: Use a coroutine to wait for the GameDataManager singleton.
    private IEnumerator Start()
    {
        // Wait until the GameDataManager singleton has been initialized.
        // This is the core fix that prevents all null reference errors.
        yield return new WaitUntil(() => GameDataManager.Instance != null);

        // Now that the manager is ready, it's safe to proceed with UI setup.
        ChangeState(UIPanel.Upgrades);
    }

    /// <summary>
    /// The single, authoritative function for changing the visible UI panel.
    /// </summary>
    private void ChangeState(UIPanel newState)
    {
        // 1. Turn ALL panels off to prevent layering issues.
        foreach (var cg in allPanelCGs)
        {
            SetCanvasGroupState(cg, false);
        }

        // 2. Turn the ONE correct panel on.
        switch (newState)
        {
            case UIPanel.Upgrades:
                SetCanvasGroupState(upgradesCG, true);
                UpdateUpgradesUI(); // Refresh data when showing
                break;
            case UIPanel.Sanctuary_Main:
                SetCanvasGroupState(sanctuaryMainCG, true);
                RefreshSanctuaryUI(); // Refresh data when showing
                break;
            case UIPanel.Sanctuary_Details:
                SetCanvasGroupState(sanctuaryDetailsCG, true);
                break;
            case UIPanel.Sanctuary_Selection:
                SetCanvasGroupState(sanctuarySelectionCG, true);
                break;
        }
    }

    private void SetCanvasGroupState(CanvasGroup cg, bool active)
    {
        if (cg == null) { Debug.LogError("A CanvasGroup is not assigned in the SurvivorCampUI inspector!", this); return; }
        cg.alpha = active ? 1f : 0f;
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }

    // --- PUBLIC BUTTON FUNCTIONS ---
    // These are the only functions your buttons should call.

    public void ShowUpgradesPanel() => ChangeState(UIPanel.Upgrades);
    public void ShowSanctuaryPanel() => ChangeState(UIPanel.Sanctuary_Main);

    public void ShowMissionDetails(MissionData missionData)
    {
        selectedMission = missionData;
        selectedSurvivorsForMission.Clear();

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
        PopulateSurvivorSelectionList();
        ChangeState(UIPanel.Sanctuary_Selection);
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
        }
    }

    public void GoToMainMenu() => SceneManager.LoadScene(mainMenuSceneName);
    public void OnStartRunClicked() => SceneManager.LoadScene(mainGameSceneName);

    #region Original Logic (Now Safe to Call)
    public void OnSurvivorToggleChanged(Survivor survivor, bool isSelected)
    {
        if (isSelected)
        {
            if (!selectedSurvivorsForMission.Contains(survivor) && selectedSurvivorsForMission.Count < selectedMission.maxSurvivors)
            {
                selectedSurvivorsForMission.Add(survivor);
            }
        }
        else { selectedSurvivorsForMission.Remove(survivor); }
        UpdateSuccessChanceDisplay();
    }

    public void UpdateUpgradesUI()
    {
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
        if (GameDataManager.Instance.gameData.sanctuarySurvivors.Count == 0) return;
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
        var idleSurvivors = GameDataManager.Instance.gameData.sanctuarySurvivors.Where(s => s.status == SurvivorStatus.Idle).ToList();
        foreach (var survivor in idleSurvivors)
        {
            GameObject newItem = Instantiate(survivorSelectionItemPrefab, survivorSelectionContent.transform);
            newItem.GetComponent<SurvivorSelectionItemUI>()?.Setup(survivor, this);
        }
    }

    private void UpdateSuccessChanceDisplay()
    {
        if (selectedMission == null) return;
        float finalChance = selectedMission.baseSuccessChance;
        if (selectedSurvivorsForMission.Count > 1) { finalChance += (selectedSurvivorsForMission.Count - 1) * selectedMission.bonusSuccessChancePerSurvivor; }
        foreach (var survivor in selectedSurvivorsForMission)
        {
            foreach (var trait in survivor.traits) { finalChance += trait.successChanceModifier; }
        }
        finalChance = Mathf.Clamp01(finalChance);
        missionSuccessChanceText.text = $"Success Chance: <color=yellow>{(finalChance * 100):F0}%</color>";
        startMissionButton.interactable = selectedSurvivorsForMission.Count > 0;
    }

    public void OnPurchaseDamageClicked() { if (GameDataManager.Instance.TryPurchaseDamageUpgrade()) UpdateUpgradesUI(); }
    public void OnPurchaseFirstAidKitClicked() { if (GameDataManager.Instance.TryPurchaseFirstAidKitUpgrade()) UpdateUpgradesUI(); }
    public void OnPurchaseScrapValueClicked() { if (GameDataManager.Instance.TryPurchaseScrapValueUpgrade()) UpdateUpgradesUI(); }
    public void OnPurchaseStartingSurvivorsClicked() { if (GameDataManager.Instance.TryPurchaseStartingSurvivorsUpgrade()) UpdateUpgradesUI(); }
    #endregion
}
