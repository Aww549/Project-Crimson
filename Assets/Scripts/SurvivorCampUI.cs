using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SurvivorCampUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject sanctuaryPanel;
    [SerializeField] private GameObject missionDetailsPanel;
    [SerializeField] private GameObject survivorSelectionPanel;

    [Header("Navigation Buttons")]
    [SerializeField] private Button backToCampButton;
    [SerializeField] private Button backToMissionListButton;
    [SerializeField] private Button backToMissionDetailsButton;
    [SerializeField] private Button startMissionButton; // This is on the survivor selection panel

    [Header("List Content")]
    [SerializeField] private Transform missionListContent;
    [SerializeField] private Transform survivorSelectionContent;
    [SerializeField] private Transform sanctuarySurvivorListContent;

    [Header("Prefabs")]
    [SerializeField] private GameObject missionListItemPrefab;
    [SerializeField] private GameObject survivorSelectionItemPrefab;
    [SerializeField] private GameObject survivorListItemPrefab;

    [Header("Mission Details")]
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionDescriptionText;
    [SerializeField] private TextMeshProUGUI missionRewardsText;
    [SerializeField] private TextMeshProUGUI missionDurationText;
    [SerializeField] private TextMeshProUGUI missionSuccessChanceText;
    [SerializeField] private Button selectSurvivorsButton; // This is on the mission details panel

    private List<GameObject> spawnedMissionItems = new List<GameObject>();
    private List<GameObject> spawnedSelectionItems = new List<GameObject>();
    private List<GameObject> spawnedSanctuarySurvivorItems = new List<GameObject>();
    private List<Survivor> selectedSurvivors = new List<Survivor>();
    private MissionData currentMission;

    private void Start()
    {
        SetupButtonListeners();
        ShowSanctuaryPanel();
        RefreshMissionList();
        RefreshSanctuarySurvivorList();
    }

    private void SetupButtonListeners()
    {
        if (backToCampButton != null) backToCampButton.onClick.AddListener(OnBackToCamp);
        if (backToMissionListButton != null) backToMissionListButton.onClick.AddListener(ShowSanctuaryPanel);
        if (backToMissionDetailsButton != null) backToMissionDetailsButton.onClick.AddListener(ShowMissionDetailsPanel);
        if (selectSurvivorsButton != null) selectSurvivorsButton.onClick.AddListener(OnSelectSurvivors);
        if (startMissionButton != null) startMissionButton.onClick.AddListener(OnStartMission);
    }

    public void OnStartMission()
    {
        if (currentMission != null && selectedSurvivors.Count > 0)
        {
            MissionController.Instance.StartMission(currentMission, selectedSurvivors);
            // Potentially clear selected survivors and return to mission list
            selectedSurvivors.Clear();
            ShowSanctuaryPanel();
        }
    }

    public void OnBackToCamp()
    {
        // It's good practice to disable the current UI before transitioning away
        // to prevent any lingering visuals or interaction.
        gameObject.SetActive(false);
        SceneTransitionManager.Instance.LoadCampScene();
    }

    private void ShowSanctuaryPanel()
    {
        sanctuaryPanel.SetActive(true);
        missionDetailsPanel.SetActive(false);
        survivorSelectionPanel.SetActive(false);
    }

    private void ShowMissionDetailsPanel()
    {
        sanctuaryPanel.SetActive(false);
        missionDetailsPanel.SetActive(true);
        survivorSelectionPanel.SetActive(false);
    }

    private void ShowSurvivorSelectionPanel()
    {
        if (sanctuaryPanel != null) sanctuaryPanel.SetActive(false);
        if (missionDetailsPanel != null) missionDetailsPanel.SetActive(false);
        if (survivorSelectionPanel != null) survivorSelectionPanel.SetActive(true);
    }

    private void RefreshMissionList()
    {
        ClearSpawnedItems(spawnedMissionItems);
        if (MissionController.Instance != null && missionListItemPrefab != null)
        {
            foreach (var mission in MissionController.Instance.allPossibleMissions)
            {
                GameObject missionItemGO = Instantiate(missionListItemPrefab, missionListContent);
                MissionListItemUI itemUI = missionItemGO.GetComponent<MissionListItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(mission, this);
                    spawnedMissionItems.Add(missionItemGO);
                }
            }
        }
    }

    public void OnSurvivorToggleChanged(Survivor survivor, bool isSelected)
    {
        if (isSelected)
        {
            if (!selectedSurvivors.Contains(survivor))
            {
                selectedSurvivors.Add(survivor);
            }
        }
        else
        {
            if (selectedSurvivors.Contains(survivor))
            {
                selectedSurvivors.Remove(survivor);
            }
        }
    }

    private void RefreshSurvivorList()
    {
        ClearSpawnedItems(spawnedSelectionItems);
        if (GameDataManager.Instance != null && survivorSelectionItemPrefab != null)
        {
            foreach (var survivor in GameDataManager.Instance.gameData.sanctuarySurvivors)
            {
                GameObject survivorItemGO = Instantiate(survivorSelectionItemPrefab, survivorSelectionContent);
                SurvivorSelectionItemUI itemUI = survivorItemGO.GetComponent<SurvivorSelectionItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(survivor, this);
                    spawnedSelectionItems.Add(survivorItemGO);
                }
            }
        }
    }

    private string GetFormattedSuccessChance(float chance)
    {
        string colorHex;
        if (chance > 0.75f)
        {
            colorHex = "#00FF00"; // Green
        }
        else if (chance > 0.4f)
        {
            colorHex = "#FFFF00"; // Yellow
        }
        else
        {
            colorHex = "#FF0000"; // Red
        }
        return $"Base Success: <color={colorHex}>{chance * 100}%</color>";
    }

    public void ShowMissionDetails(MissionData missionData)
    {
        if (missionData != null)
        {
            currentMission = missionData;
            missionNameText.text = missionData.missionName;
            missionDescriptionText.text = missionData.description;
            missionRewardsText.text = $"Reward: {missionData.baseRewardAmount} {missionData.rewardType}";
            missionDurationText.text = $"Duration: {missionData.durationHours} Hours";
            missionSuccessChanceText.text = GetFormattedSuccessChance(missionData.baseSuccessChance);
            ShowMissionDetailsPanel();
        }
    }

    public void OnSelectSurvivors()
    {
        Debug.Log("OnSelectSurvivors called. Attempting to show survivor selection panel.");
        ShowSurvivorSelectionPanel();
        RefreshSurvivorList();
    }

    private void ClearSpawnedItems(List<GameObject> items)
    {
        foreach (var item in items)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        items.Clear();
    }

    private void RefreshSanctuarySurvivorList()
    {
        ClearSpawnedItems(spawnedSanctuarySurvivorItems);
        if (GameDataManager.Instance != null && survivorListItemPrefab != null)
        {
            foreach (var survivor in GameDataManager.Instance.gameData.sanctuarySurvivors)
            {
                GameObject survivorItemGO = Instantiate(survivorListItemPrefab, sanctuarySurvivorListContent);
                SurvivorListItemUI itemUI = survivorItemGO.GetComponent<SurvivorListItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(survivor);
                    spawnedSanctuarySurvivorItems.Add(survivorItemGO);
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (backToCampButton != null) backToCampButton.onClick.RemoveAllListeners();
        if (backToMissionListButton != null) backToMissionListButton.onClick.RemoveAllListeners();
        if (backToMissionDetailsButton != null) backToMissionDetailsButton.onClick.RemoveAllListeners();
        if (selectSurvivorsButton != null) selectSurvivorsButton.onClick.RemoveAllListeners();
        if (startMissionButton != null) startMissionButton.onClick.RemoveAllListeners();

        ClearSpawnedItems(spawnedMissionItems);
        ClearSpawnedItems(spawnedSelectionItems);
        ClearSpawnedItems(spawnedSanctuarySurvivorItems);
    }
}