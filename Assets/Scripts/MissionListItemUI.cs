using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MissionListItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDurationText;
    public TextMeshProUGUI missionRewardText;
    public Button selectMissionButton;

    private MissionData assignedMission;
    private SurvivorCampUI survivorCampUI;

    public void Setup(MissionData missionData, SurvivorCampUI owningUI)
    {
        assignedMission = missionData;
        survivorCampUI = owningUI;

        if (missionNameText != null) missionNameText.text = missionData.missionName;
        if (missionDurationText != null) missionDurationText.text = $"{missionData.durationHours} Hours";
        if (missionRewardText != null) missionRewardText.text = $"Reward: {missionData.baseRewardAmount} {missionData.rewardType}";

        if (selectMissionButton != null)
        {
            selectMissionButton.onClick.RemoveAllListeners();
            selectMissionButton.onClick.AddListener(OnMissionClicked);
        }
    }

    public void UpdateStatus()
    {
        if (assignedMission == null || GameDataManager.Instance == null) return;

        bool isInProgress = GameDataManager.Instance.gameData.activeMissions
            .Any(m => m.missionDataName == assignedMission.name);

        if (selectMissionButton != null)
        {
            selectMissionButton.interactable = !isInProgress;
        }
    }

    private void OnMissionClicked()
    {
        if (survivorCampUI != null)
        {
            survivorCampUI.ShowMissionDetails(assignedMission);
        }
    }
}
