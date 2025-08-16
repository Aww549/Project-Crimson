using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SurvivorSelectionItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Toggle selectionToggle;
    public TextMeshProUGUI survivorNameText;
    public TextMeshProUGUI survivorTraitsText;

    private Survivor assignedSurvivor;
    private SurvivorCampUI survivorCampUI; // Changed from SanctuaryUI

    /// <summary>
    /// Populates the UI with a survivor's data and sets up the toggle listener.
    /// </summary>
    public void Setup(Survivor survivor, SurvivorCampUI owningUI) // Changed from SanctuaryUI
    {
        assignedSurvivor = survivor;
        survivorCampUI = owningUI;

        survivorNameText.text = survivor.survivorName;

        if (survivor.traits != null && survivor.traits.Count > 0)
        {
            survivorTraitsText.text = string.Join(", ", survivor.traits.ConvertAll(t => t.traitName));
        }
        else
        {
            survivorTraitsText.text = "<i>No Traits</i>";
        }

        selectionToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    /// <summary>
    /// Called automatically when the toggle is clicked.
    /// </summary>
    private void OnToggleChanged(bool isOn)
    {
        survivorCampUI.OnSurvivorToggleChanged(assignedSurvivor, isOn);
    }

    void OnDestroy()
    {
        if (selectionToggle != null)
        {
            selectionToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }
}
