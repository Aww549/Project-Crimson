using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SurvivorCampUI : MonoBehaviour
{
    #region Variables
    [Header("--- Main Panels ---")]
    public GameObject upgradesPanel;
    public GameObject sanctuaryPanel;
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
    [Header("--- Sanctuary: Roster ---")]
    public GameObject survivorListContent;
    public GameObject survivorListItemPrefab;
    [Header("--- Sanctuary: Missions ---")]
    public GameObject missionListContent;
    public GameObject missionListItemPrefab;
    [Header("--- Sanctuary: Details Panel ---")]
    public GameObject missionDetailsPanel;
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDescriptionText;
    public TextMeshProUGUI missionRewardText;
    public TextMeshProUGUI missionDurationText;
    public TextMeshProUGUI missionSuccessChanceText;
    [Header("--- Sanctuary: Selection Panel ---")]
    public GameObject survivorSelectionPanel;
    public GameObject survivorSelectionContent;
    public GameObject survivorSelectionItemPrefab;
    public Button startMissionButton;
    private MissionData selectedMission;
    private List<Survivor> selectedSurvivorsForMission = new List<Survivor>();
    #endregion

    void Start()
    {
        missionDetailsPanel.SetActive(false);
        survivorSelectionPanel.SetActive(false);
        ShowUpgradesPanel();
    }

    public void ShowUpgradesPanel()
    {
        upgradesPanel.SetActive(true);
        sanctuaryPanel.SetActive(false);
        UpdateUpgradesUI();
    }

    public void ShowSanctuaryPanel()
    {
        upgradesPanel.SetActive(false);
        sanctuaryPanel.SetActive(true);
        RefreshSanctuaryUI();
    }

    public void CloseMissionDetails()
    {
        missionDetailsPanel.SetActive(false);
        selectedMission = null;
    }

    public void OpenSurvivorSelection()
    {
        if (selectedMission == null) return;
        missionDetailsPanel.SetActive(false);
        survivorSelectionPanel.SetActive(true);
        PopulateSurvivorSelectionList();
    }

    public void CloseSurvivorSelection()
    {
        survivorSelectionPanel.SetActive(false);
        if (selectedMission != null)
        {
            missionDetailsPanel.SetActive(true);
        }
    }

    public void OnStartMissionClicked()
    {
        if (selectedMission != null && selectedSurvivorsForMission.Count > 0)
        {
            MissionController.Instance.StartMission(selectedMission, selectedSurvivorsForMission);
            survivorSelectionPanel.SetActive(false);
            missionDetailsPanel.SetActive(false);
            selectedMission = null;
            RefreshSanctuaryUI();
        }
    }

    #region Unchanged Methods
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
        if (GameDataManager.Instance == null || GameDataManager.Instance.gameData.sanctuarySurvivors.Count == 0) { return; }
        foreach (Survivor survivor in GameDataManager.Instance.gameData.sanctuarySurvivors)
        {
            GameObject newItem = Instantiate(survivorListItemPrefab, survivorListContent.transform);
            var itemUI = newItem.GetComponent<SurvivorListItemUI>();
            if (itemUI != null) { itemUI.Setup(survivor); }
        }
    }

    private void PopulateMissionList()
    {
        foreach (Transform child in missionListContent.transform) { Destroy(child.gameObject); }
        var allMissions = Resources.LoadAll<MissionData>("Missions").ToList();
        if (allMissions.Count == 0) { return; }
        foreach (var missionData in allMissions)
        {
            GameObject newItem = Instantiate(missionListItemPrefab, missionListContent.transform);
            var itemUI = newItem.GetComponent<MissionListItemUI>();
            if (itemUI != null) { itemUI.Setup(missionData, this); }
        }
    }

    public void ShowMissionDetails(MissionData missionData)
    {
        selectedMission = missionData;
        missionDetailsPanel.SetActive(true);
        selectedSurvivorsForMission.Clear();
        UpdateSuccessChanceDisplay();
        missionNameText.text = missionData.missionName;
        missionDescriptionText.text = missionData.description;
        missionRewardText.text = $"Base Reward: {missionData.baseRewardAmount} {missionData.rewardType}";
        missionDurationText.text = $"Duration: {missionData.durationHours} Hours";
    }

    private void PopulateSurvivorSelectionList()
    {
        foreach (Transform child in survivorSelectionContent.transform) { Destroy(child.gameObject); }
        var idleSurvivors = GameDataManager.Instance.gameData.sanctuarySurvivors.Where(s => s.status == SurvivorStatus.Idle).ToList();
        foreach (var survivor in idleSurvivors)
        {
            GameObject newItem = Instantiate(survivorSelectionItemPrefab, survivorSelectionContent.transform);
            var itemUI = newItem.GetComponent<SurvivorSelectionItemUI>();
            itemUI.Setup(survivor, this);
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
            if (selectedSurvivorsForMission.Contains(survivor))
            {
                selectedSurvivorsForMission.Remove(survivor);
            }
        }
        UpdateSuccessChanceDisplay();
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

    public void GoToMainMenu() { SceneManager.LoadScene(mainMenuSceneName); }
    public void OnStartRunClicked() { SceneManager.LoadScene(mainGameSceneName); }
    public void OnPurchaseDamageClicked() { if (GameDataManager.Instance.TryPurchaseDamageUpgrade()) UpdateUpgradesUI(); }
    public void OnPurchaseFirstAidKitClicked() { if (GameDataManager.Instance.TryPurchaseFirstAidKitUpgrade()) UpdateUpgradesUI(); }
    public void OnPurchaseScrapValueClicked() { if (GameDataManager.Instance.TryPurchaseScrapValueUpgrade()) UpdateUpgradesUI(); }
    public void OnPurchaseStartingSurvivorsClicked() { if (GameDataManager.Instance.TryPurchaseStartingSurvivorsUpgrade()) UpdateUpgradesUI(); }
    #endregion
}
