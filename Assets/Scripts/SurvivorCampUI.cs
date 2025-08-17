using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SurvivorCampUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject survivorListPanel;
    [SerializeField] private GameObject missionListPanel;
    [SerializeField] private GameObject missionDetailsPanel;
    [SerializeField] private GameObject survivorSelectionPanel;

    [Header("Navigation Buttons")]
    [SerializeField] private Button backToCampButton;
    [SerializeField] private Button backToMissionListButton;
    [SerializeField] private Button backToMissionDetailsButton;
    [SerializeField] private Button confirmMissionButton;

    [Header("List Content")]
    [SerializeField] private Transform survivorListContent;
    [SerializeField] private Transform missionListContent;
    [SerializeField] private Transform survivorSelectionContent;

    [Header("Prefabs")]
    [SerializeField] private GameObject survivorListItemPrefab;
    [SerializeField] private GameObject missionListItemPrefab;
    [SerializeField] private GameObject survivorSelectionItemPrefab;

    [Header("Mission Details")]
    [SerializeField] private Text missionNameText;
    [SerializeField] private Text missionDescriptionText;
    [SerializeField] private Text missionRewardsText;
    [SerializeField] private Button startMissionButton;

    private List<GameObject> spawnedSurvivorItems = new List<GameObject>();
    private List<GameObject> spawnedMissionItems = new List<GameObject>();
    private List<GameObject> spawnedSelectionItems = new List<GameObject>();
    private List<Survivor> selectedSurvivors = new List<Survivor>();
    private MissionData currentMission;

    private void Start()
    {
        SetupButtonListeners();
        ShowMissionListPanel();
        RefreshMissionList();
    }

    private void SetupButtonListeners()
    {
        backToCampButton.onClick.AddListener(OnBackToCamp);
        backToMissionListButton.onClick.AddListener(ShowMissionListPanel);
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
            ShowMissionListPanel();
        }
    }

    private void OnBackToCamp()
    {
        SceneTransitionManager.Instance.LoadCampScene();
    }

    private void ShowMissionListPanel()
    {
        survivorListPanel.SetActive(false);
        missionListPanel.SetActive(true);
        missionDetailsPanel.SetActive(false);
        survivorSelectionPanel.SetActive(false);
    }

    private void ShowMissionDetailsPanel()
    {
        survivorListPanel.SetActive(false);
        missionListPanel.SetActive(false);
        missionDetailsPanel.SetActive(true);
        survivorSelectionPanel.SetActive(false);
    }

    private void ShowSurvivorSelectionPanel()
    {
        survivorListPanel.SetActive(false);
        missionListPanel.SetActive(false);
        missionDetailsPanel.SetActive(false);
        survivorSelectionPanel.SetActive(true);
    }

    private void RefreshMissionList()
    {
        ClearSpawnedItems(spawnedMissionItems);
        if (MissionController.Instance != null && missionListItemPrefab != null)
        {
            foreach (var mission in MissionController.Instance.availableMissions)
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
            foreach (var survivor in GameDataManager.Instance.gameData.survivors)
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

        ClearSpawnedItems(spawnedSurvivorItems);
        ClearSpawnedItems(spawnedMissionItems);
        ClearSpawnedItems(spawnedSelectionItems);
    }
}