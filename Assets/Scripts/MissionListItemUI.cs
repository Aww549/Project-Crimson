using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionListItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDurationText;
    public TextMeshProUGUI missionRewardText;

    [Header("State UI")]
    public Button selectButton;
    public GameObject inProgressGroup;
    public TextMeshProUGUI stateText;
    public Button claimButton;
    public TextMeshProUGUI inProgressTimerText;

    private MissionData assignedMission;
    private SurvivorCampUI survivorCampUI;

    private float nextRefreshAt = 0f;

    public void Setup(MissionData missionData, SurvivorCampUI owningUI)
    {
        assignedMission = missionData;
        survivorCampUI = owningUI;

        if (missionNameText != null) missionNameText.text = missionData.missionName;
        if (missionDurationText != null) missionDurationText.text = $"{missionData.durationHours} Hours";
        if (missionRewardText != null) missionRewardText.text = $"Reward: {missionData.baseRewardAmount} {missionData.rewardType}";

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnMissionClicked);
        }
        if (claimButton != null)
        {
            // IMPORTANT: Ensure there is NO persistent OnClick for claimButton in the Inspector.
            // This removes only runtime listeners; persistent (Inspector) ones must be cleared manually.
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(OnClaimClicked);
            claimButton.gameObject.SetActive(false);
            claimButton.interactable = true;
        }

        if (inProgressGroup != null) inProgressGroup.SetActive(false);
        RefreshState();
    }

    void OnEnable()
    {
        RefreshState();
    }

    void Update()
    {
        if (Time.unscaledTime >= nextRefreshAt)
        {
            nextRefreshAt = Time.unscaledTime + 0.5f;
            RefreshState();
        }
    }

    public void OnMissionClicked()
    {
        if (survivorCampUI != null)
        {
            survivorCampUI.ShowMissionDetails(assignedMission);
        }
    }

    private void OnClaimClicked()
    {
        if (GameDataManager.Instance == null || GameDataManager.Instance.gameData == null || assignedMission == null)
            return;

        // Immediately block further clicks on this button to prevent double-claims
        if (claimButton != null)
        {
            claimButton.interactable = false;
        }
        if (stateText != null)
        {
            stateText.text = "Processing...";
        }

        var active = GameDataManager.Instance.gameData.activeMissions
            .FirstOrDefault(m => m.missionDataName == assignedMission.name && m.isReadyToClaim);

        if (active == null)
        {
            // Nothing to claim (already processed by another click/poller)
            if (survivorCampUI != null) survivorCampUI.RefreshAllListsPublic();
            return;
        }

        MissionController.Instance.ClaimMission(active.missionId);

        // Refresh whole panel so survivor list and missions update and the item is rebuilt
        if (survivorCampUI != null)
        {
            survivorCampUI.RefreshAllListsPublic();
        }
    }

    private void RefreshState()
    {
        if (assignedMission == null || GameDataManager.Instance == null || GameDataManager.Instance.gameData == null)
        {
            SetAvailableState();
            return;
        }

        var active = GameDataManager.Instance.gameData.activeMissions
            .FirstOrDefault(m => m.missionDataName == assignedMission.name);

        if (active == null)
        {
            SetAvailableState();
            return;
        }

        if (active.isReadyToClaim)
        {
            SetReadyToClaimState();
        }
        else
        {
            SetInProgressState(active);
        }
    }

    private void SetAvailableState()
    {
        if (inProgressGroup != null) inProgressGroup.SetActive(false);
        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(false);
            claimButton.interactable = true;
        }
        if (stateText != null) stateText.text = string.Empty;
        if (inProgressTimerText != null) inProgressTimerText.text = string.Empty;
        if (selectButton != null) selectButton.interactable = true;
    }

    private void SetInProgressState(ActiveMission active)
    {
        if (inProgressGroup != null) inProgressGroup.SetActive(true);
        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(false);
            claimButton.interactable = false;
        }
        if (selectButton != null) selectButton.interactable = false;
        if (stateText != null) stateText.text = "In Progress";

        if (inProgressTimerText != null)
        {
            long remainingTicks = active.missionEndTimeTicks - DateTime.UtcNow.Ticks;
            if (remainingTicks < 0) remainingTicks = 0;
            var ts = TimeSpan.FromTicks(remainingTicks);
            inProgressTimerText.text = $"{(int)ts.TotalMinutes:00}:{ts.Seconds:00}";
        }
    }

    private void SetReadyToClaimState()
    {
        if (inProgressGroup != null) inProgressGroup.SetActive(true);
        if (stateText != null) stateText.text = "Ready to Claim";
        if (inProgressTimerText != null) inProgressTimerText.text = "00:00";
        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(true);
            claimButton.interactable = true;
        }
        if (selectButton != null) selectButton.interactable = false;
    }
}