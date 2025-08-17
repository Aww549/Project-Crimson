using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System; // Required for TimeSpan

public class SurvivorListItemUI : MonoBehaviour
{
    [Header("Card Objects")]
    public GameObject frontPanel;
    public GameObject backPanel;
    public GameObject onMissionOverlay; // The greyed-out panel with the timer

    [Header("Front Panel UI")]
    public TextMeshProUGUI survivorNameText;
    public Image survivorIcon; // We can add an icon or portrait here later

    [Header("Back Panel UI")]
    public TextMeshProUGUI back_survivorNameText;
    public TextMeshProUGUI back_traitsText;

    [Header("On Mission Overlay UI")]
    public TextMeshProUGUI missionTimerText;

    private Survivor assignedSurvivor;
    private bool isFlipped = false;

    void Awake()
    {
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (frontPanel == null) Debug.LogError("SurvivorListItemUI: frontPanel is not assigned.");
        if (backPanel == null) Debug.LogError("SurvivorListItemUI: backPanel is not assigned.");
        if (onMissionOverlay == null) Debug.LogError("SurvivorListItemUI: onMissionOverlay is not assigned.");
        if (survivorNameText == null) Debug.LogError("SurvivorListItemUI: survivorNameText is not assigned.");
        if (back_survivorNameText == null) Debug.LogError("SurvivorListItemUI: back_survivorNameText is not assigned.");
        if (back_traitsText == null) Debug.LogError("SurvivorListItemUI: back_traitsText is not assigned.");
        if (missionTimerText == null) Debug.LogError("SurvivorListItemUI: missionTimerText is not assigned.");
    }

    /// <summary>
    /// Populates the UI elements with data from a specific survivor.
    /// </summary>
    public void Setup(Survivor survivor)
    {
        assignedSurvivor = survivor;
        isFlipped = false;

        if (survivor == null)
        {
            Debug.LogError("Setup called with a null survivor.");
            return;
        }

        // --- Populate Shared Info ---
        if(survivorNameText != null) survivorNameText.text = survivor.survivorName;
        if(back_survivorNameText != null) back_survivorNameText.text = survivor.survivorName;

        // --- Populate Traits on the Back ---
        if(back_traitsText != null)
        {
            if (survivor.traits == null || survivor.traits.Count == 0)
            {
                back_traitsText.text = "<i>No Traits</i>";
            }
            else
            {
                StringBuilder traitsBuilder = new StringBuilder();
                foreach (var trait in survivor.traits)
                {
                    traitsBuilder.AppendLine($"<b>{trait.traitName}</b>: {trait.description}");
                }
                back_traitsText.text = traitsBuilder.ToString();
            }
        }

        // --- Update View Based on Status ---
        UpdateStatusView();
    }

    /// <summary>
    /// Updates the card's visual state based on the survivor's status.
    /// </summary>
    public void UpdateStatusView()
    {
        if (assignedSurvivor == null) return;

        if (assignedSurvivor.status == SurvivorStatus.OnMission)
        {
            if(frontPanel != null) frontPanel.SetActive(true);
            if(backPanel != null) backPanel.SetActive(false);
            if(onMissionOverlay != null) onMissionOverlay.SetActive(true);
            // We'll add the timer logic here later
        }
        else // Idle or Wounded
        {
            if(onMissionOverlay != null) onMissionOverlay.SetActive(false);
            // Set initial flipped state
            if(frontPanel != null) frontPanel.SetActive(!isFlipped);
            if(backPanel != null) backPanel.SetActive(isFlipped);
        }
    }

    /// <summary>
    /// Called when the player clicks on this survivor item. Flips the card.
    /// </summary>
    public void OnSurvivorClicked()
    {
        // Can only flip if the survivor is NOT on a mission
        if (assignedSurvivor.status != SurvivorStatus.OnMission)
        {
            isFlipped = !isFlipped;
            frontPanel.SetActive(!isFlipped);
            backPanel.SetActive(isFlipped);
        }
    }
}
