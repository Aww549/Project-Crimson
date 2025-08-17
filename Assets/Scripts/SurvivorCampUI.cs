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
    [SerializeField] private Button confirmMissionButton;

    [Header("List Content")]
    [SerializeField] private Transform missionListContent;
    [SerializeField] private Transform survivorSelectionContent;

    [Header("Prefabs")]
    [SerializeField] private GameObject missionListItemPrefab;
    [SerializeField] private GameObject survivorSelectionItemPrefab;

    [Header("Mission Details")]
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionDescriptionText;
    [SerializeField] private TextMeshProUGUI missionRewardsText;
    [SerializeField] private Button startMissionButton;

    private List<GameObject> spawnedMissionItems = new List<GameObject>();
    private List<GameObject> spawnedSelectionItems = new List<GameObject>();
    private List<Survivor> selectedSurvivors = new List<Survivor>();
    private MissionData currentMission;

    private void Start()
    {
        SetupButtonListeners();
        ShowSanctuaryPanel();
        RefreshMissionList();
    }

    private void SetupButtonListeners()
    {
        backToCampButton.onClick.AddListener(OnBackToCamp);
        backToMissionListButton.onClick.AddListener(ShowSanctuaryPanel);
        backToMissionDetailsButton.onClick.AddListener(ShowMissionDetailsPanel);
        startMissionButton.onClick.AddListener(OnStartMission);
        if (confirmMissionButton != null) confirmMissionButton.onClick.AddListener(OnConfirmMission);
    }

    private void OnConfirmMission()
    {
        if (currentMission != null && selectedSurvivors.Count > 0)
        {
            MissionController.Instance.StartMission(currentMission, selectedSurvivors);
            // Potentially clear selected survivors and return to mission list
            selectedSurvivors.Clear();
            ShowSanctuaryPanel();
        }
    }

    private void OnBackToCamp()
    {
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
        sanctuaryPanel.SetActive(false);
        missionDetailsPanel.SetActive(false);
        survivorSelectionPanel.SetActive(true);
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

    public void ShowMissionDetails(MissionData missionData)
    {
        if (missionData != null)
        {
            currentMission = missionData;
            missionNameText.text = missionData.missionName;
            missionDescriptionText.text = missionData.description;
            missionRewardsText.text = $"Rewards: {missionData.baseRewardAmount} {missionData.rewardType}";
            ShowMissionDetailsPanel();
        }
    }

    private void OnStartMission()
    {
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

    private void OnDestroy()
    {
        // Clean up listeners
        if (backToCampButton != null) backToCampButton.onClick.RemoveAllListeners();
        if (backToMissionListButton != null) backToMissionListButton.onClick.RemoveAllListeners();
        if (backToMissionDetailsButton != null) backToMissionDetailsButton.onClick.RemoveAllListeners();
        if (startMissionButton != null) startMissionButton.onClick.RemoveAllListeners();
        if (confirmMissionButton != null) confirmMissionButton.onClick.RemoveAllListeners();

        ClearSpawnedItems(spawnedMissionItems);
        ClearSpawnedItems(spawnedSelectionItems);
    }
}