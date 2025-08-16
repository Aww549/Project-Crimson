using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MissionsUI : MonoBehaviour
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
        if (MissionController.Instance != null)
        {
            // Add your mission list population logic here
        }
    }

    private void RefreshSurvivorList()
    {
        ClearSpawnedItems(spawnedSurvivorItems);
        if (SanctuaryManager.Instance != null)
        {
            // Add your survivor list population logic here
        }
    }

    private void OnMissionSelected(MissionData missionData)
    {
        if (missionData != null)
        {
            missionNameText.text = missionData.name;
            missionDescriptionText.text = missionData.description;
            missionRewardsText.text = $"Rewards: {missionData.rewards}";
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

        ClearSpawnedItems(spawnedSurvivorItems);
        ClearSpawnedItems(spawnedMissionItems);
        ClearSpawnedItems(spawnedSelectionItems);
    }
}