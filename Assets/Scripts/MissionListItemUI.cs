using UnityEngine;
using TMPro;

public class MissionListItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDurationText;
    public TextMeshProUGUI missionRewardText;

    private MissionData assignedMission;
    private SurvivorCampUI survivorCampUI; // Changed from SanctuaryUI

    /// <summary>
    /// Populates the UI elements with data from a specific mission.
    /// </summary>
    public void Setup(MissionData missionData, SurvivorCampUI owningUI) // Changed from SanctuaryUI
    {
        assignedMission = missionData;
        survivorCampUI = owningUI;

        missionNameText.text = missionData.missionName;
        missionDurationText.text = $"{missionData.durationHours} Hours";
        missionRewardText.text = $"Reward: {missionData.baseRewardAmount} {missionData.rewardType}";
    }

    /// <summary>
    /// Called when the player clicks this mission item. Tells the main UI to open the details panel.
    /// </summary>
    public void OnMissionClicked()
    {
        if (survivorCampUI != null)
        {
            survivorCampUI.ShowMissionDetails(assignedMission);
        }
    }
}
