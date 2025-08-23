using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Displays the results of a completed mission, showing success/failure, rewards gained, and any survivors lost.
/// </summary>
public class MissionResultsPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI rewardsText;
    [SerializeField] private TextMeshProUGUI survivorsText;
    [SerializeField] private Button closeButton;

    void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the panel with success information.
    /// </summary>
    public void ShowSuccessResult(string missionName, MissionRewardType rewardType, int rewardAmount)
    {
        titleText.text = "Mission Complete";
        resultText.text = $"Your team has successfully completed the \"{missionName}\" mission!";

        string rewardTypeText = rewardType == MissionRewardType.Materials ? "Materials" : "Scrap";
        rewardsText.text = $"Rewards: {rewardAmount} {rewardTypeText}";

        survivorsText.text = "All survivors have returned safely.";

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows the panel with failure information, including which survivors were lost.
    /// </summary>
    public void ShowFailureResult(string missionName, List<string> lostSurvivorNames)
    {
        titleText.text = "Mission Failed";
        resultText.text = $"Your team was unable to complete the \"{missionName}\" mission.";
        rewardsText.text = "Rewards: None";

        if (lostSurvivorNames.Count > 0)
        {
            string survivorsLostText = "Survivors lost:";
            foreach (var name in lostSurvivorNames)
            {
                survivorsLostText += $"\n• {name}";
            }
            survivorsText.text = survivorsLostText;
        }
        else
        {
            survivorsText.text = "By some miracle, all survivors managed to return safely.";
        }

        gameObject.SetActive(true);
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);

        // Reload the scene to refresh all lists
        SceneManager.LoadScene(SceneTransitionManager.MISSIONS_SCENE, LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}