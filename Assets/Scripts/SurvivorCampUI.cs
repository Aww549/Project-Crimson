using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// This script has been rewritten from scratch to match the scene hierarchy provided by the user.
// The variable names should now directly correspond to the names of the GameObjects
// in the scene, making them easy to assign in the Inspector.
public class SurvivorCampUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject SanctuaryMainPanel;
    [SerializeField] private GameObject MissionDetailsPanel;
    [SerializeField] private GameObject SurvivorSelectionPanel;

    [Header("Main Navigation Buttons")]
    [SerializeField] private Button BackToUpgradesButton;

    [Header("Mission Detail Buttons")]
    [SerializeField] private Button SelectSurvivors_Button;
    [SerializeField] private Button Close_Button;

    [Header("Survivor Selection Buttons")]
    [SerializeField] private Button StartMission_Button;
    [SerializeField] private Button Cancel_Button;

    [Header("List Content Transforms")]
    [SerializeField] private Transform MissionList_Content;
    [SerializeField] private Transform SurvivorRoster_Content;
    [SerializeField] private Transform Selection_Content; // Survivor selection list

    [Header("Prefabs")]
    [SerializeField] private GameObject missionListItemPrefab;
    [SerializeField] private GameObject survivorListItemPrefab; // For the main roster
    [SerializeField] private GameObject survivorSelectionItemPrefab; // For the mission selection list

    [Header("Mission Details UI")]
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionDescriptionText;
    [SerializeField] private TextMeshProUGUI missionDurationText;
    [SerializeField] private TextMeshProUGUI missionRewardText;
    [SerializeField] private TextMeshProUGUI missionSuccessChanceText;

    // Private Data
    private List<GameObject> spawnedMissionItems = new List<GameObject>();
    private List<GameObject> spawnedSanctuarySurvivorItems = new List<GameObject>();
    private List<GameObject> spawnedSelectionItems = new List<GameObject>();
    private List<Survivor> selectedSurvivors = new List<Survivor>();
    private MissionData currentMission;

    void Start()
    {
        SetupButtonListeners();
        ShowSanctuaryPanel(); // Start on the main panel
        RefreshAllLists();
    }

    private void SetupButtonListeners()
    {
        // Navigation
        if (BackToUpgradesButton != null) BackToUpgradesButton.onClick.AddListener(OnBackToCamp);
        if (Close_Button != null) Close_Button.onClick.AddListener(ShowSanctuaryPanel);
        if (Cancel_Button != null) Cancel_Button.onClick.AddListener(ShowMissionDetailsPanel);

        // Actions
        if (SelectSurvivors_Button != null) SelectSurvivors_Button.onClick.AddListener(OnSelectSurvivors);
        if (StartMission_Button != null) StartMission_Button.onClick.AddListener(OnStartMission);
    }

    private void RefreshAllLists()
    {
        RefreshMissionList();
        RefreshSanctuarySurvivorList();
    }

    // --- Panel Navigation ---

    private void ShowSanctuaryPanel()
    {
        if (SanctuaryMainPanel != null) SanctuaryMainPanel.SetActive(true);
        if (MissionDetailsPanel != null) MissionDetailsPanel.SetActive(false);
        if (SurvivorSelectionPanel != null) SurvivorSelectionPanel.SetActive(false);
    }

    private void ShowMissionDetailsPanel()
    {
        if (SanctuaryMainPanel != null) SanctuaryMainPanel.SetActive(false);
        if (MissionDetailsPanel != null) MissionDetailsPanel.SetActive(true);
        if (SurvivorSelectionPanel != null) SurvivorSelectionPanel.SetActive(false);
    }

    private void ShowSurvivorSelectionPanel()
    {
        if (SanctuaryMainPanel != null) SanctuaryMainPanel.SetActive(false);
        if (MissionDetailsPanel != null) MissionDetailsPanel.SetActive(false);
        if (SurvivorSelectionPanel != null) SurvivorSelectionPanel.SetActive(true);
    }

    // --- Public Methods (Called by other UI elements) ---

    public void ShowMissionDetails(MissionData missionData)
    {
        currentMission = missionData;
        if (missionNameText != null) missionNameText.text = missionData.missionName;
        if (missionDescriptionText != null) missionDescriptionText.text = missionData.description;
        if (missionRewardText != null) missionRewardText.text = $"Reward: {missionData.baseRewardAmount} {missionData.rewardType}";
        if (missionDurationText != null) missionDurationText.text = $"Duration: {missionData.durationHours} Hours";
        if (missionSuccessChanceText != null) missionSuccessChanceText.text = GetFormattedSuccessChance(missionData.baseSuccessChance);

        ShowMissionDetailsPanel();
    }

    public void OnSurvivorToggleChanged(Survivor survivor, bool isSelected)
    {
        if (isSelected)
        {
            if (!selectedSurvivors.Contains(survivor)) selectedSurvivors.Add(survivor);
        }
        else
        {
            if (selectedSurvivors.Contains(survivor)) selectedSurvivors.Remove(survivor);
        }
    }

    // --- Private UI Logic ---

    private void OnBackToCamp()
    {
        gameObject.SetActive(false);
        SceneTransitionManager.Instance.LoadCampScene();
    }

    private void OnSelectSurvivors()
    {
        ShowSurvivorSelectionPanel();
        RefreshSurvivorSelectionList();
    }

    private void OnStartMission()
    {
        if (currentMission != null && selectedSurvivors.Count > 0)
        {
            MissionController.Instance.StartMission(currentMission, selectedSurvivors);
            selectedSurvivors.Clear();
            ShowSanctuaryPanel();
        }
    }

    private void RefreshMissionList()
    {
        ClearSpawnedItems(spawnedMissionItems);
        if (GameDataManager.Instance != null && missionListItemPrefab != null && MissionList_Content != null)
        {
            foreach (var mission in MissionController.Instance.allPossibleMissions)
            {
                GameObject itemGO = Instantiate(missionListItemPrefab, MissionList_Content);
                var itemUI = itemGO.GetComponent<MissionListItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(mission, this); // 'this' is the SurvivorCampUI instance
                    spawnedMissionItems.Add(itemGO);
                }
            }
        }
    }

    private void RefreshSanctuarySurvivorList()
    {
        ClearSpawnedItems(spawnedSanctuarySurvivorItems);
        if (GameDataManager.Instance != null && survivorListItemPrefab != null && SurvivorRoster_Content != null)
        {
            foreach (var survivor in GameDataManager.Instance.gameData.sanctuarySurvivors)
            {
                GameObject itemGO = Instantiate(survivorListItemPrefab, SurvivorRoster_Content);
                var itemUI = itemGO.GetComponent<SurvivorListItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(survivor);
                    spawnedSanctuarySurvivorItems.Add(itemGO);
                }
            }
        }
    }

    private void RefreshSurvivorSelectionList()
    {
        ClearSpawnedItems(spawnedSelectionItems);
        if (GameDataManager.Instance != null && survivorSelectionItemPrefab != null && Selection_Content != null)
        {
            foreach (var survivor in GameDataManager.Instance.gameData.sanctuarySurvivors)
            {
                // Later, we might want to filter out survivors who are already on a mission
                GameObject itemGO = Instantiate(survivorSelectionItemPrefab, Selection_Content);
                var itemUI = itemGO.GetComponent<SurvivorSelectionItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(survivor, this);
                    spawnedSelectionItems.Add(itemGO);
                }
            }
        }
    }

    private void ClearSpawnedItems(List<GameObject> items)
    {
        foreach (var item in items)
        {
            if (item != null) Destroy(item);
        }
        items.Clear();
    }

    private string GetFormattedSuccessChance(float chance)
    {
        string colorHex = (chance > 0.75f) ? "#00FF00" : (chance > 0.4f) ? "#FFFF00" : "#FF0000";
        return $"Base Success: <color={colorHex}>{chance * 100}%</color>";
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (BackToUpgradesButton != null) BackToUpgradesButton.onClick.RemoveAllListeners();
        if (Close_Button != null) Close_Button.onClick.RemoveAllListeners();
        if (Cancel_Button != null) Cancel_Button.onClick.RemoveAllListeners();
        if (SelectSurvivors_Button != null) SelectSurvivors_Button.onClick.RemoveAllListeners();
        if (StartMission_Button != null) StartMission_Button.onClick.RemoveAllListeners();
    }
}