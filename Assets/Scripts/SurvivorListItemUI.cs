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

    /// <summary>
    /// Populates the UI elements with data from a specific survivor.
    /// </summary>
    public void Setup(Survivor survivor)
    {
        assignedSurvivor = survivor;
        isFlipped = false;

        // --- Populate Shared Info ---
        survivorNameText.text = survivor.survivorName;
        back_survivorNameText.text = survivor.survivorName;

        // --- Populate Traits on the Back ---
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

        // --- Update View Based on Status ---
        UpdateStatusView();
    }

    /// <summary>
    /// Updates the card's visual state based on the survivor's status.
    /// </summary>
    public void UpdateStatusView()
    {
        if (assignedSurvivor.status == SurvivorStatus.OnMission)
        {
            frontPanel.SetActive(true); // Always show the front when on a mission
            backPanel.SetActive(false);
            onMissionOverlay.SetActive(true);
            // We'll add the timer logic here later
        }
        else // Idle or Wounded
        {
            onMissionOverlay.SetActive(false);
            // Set initial flipped state
            frontPanel.SetActive(!isFlipped);
            backPanel.SetActive(isFlipped);
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
