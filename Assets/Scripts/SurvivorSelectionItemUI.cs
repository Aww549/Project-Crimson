using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

        if (survivorNameText != null)
        {
            survivorNameText.text = survivor.survivorName;
        }

        if (survivorTraitsText != null)
        {
            if (survivor.traits != null && survivor.traits.Count > 0)
            {
                // Use LINQ to filter out null traits before trying to access their properties.
                var validTraits = survivor.traits.Where(t => t != null).Select(t => t.traitName);
                survivorTraitsText.text = string.Join(", ", validTraits);
            }
            else
            {
                survivorTraitsText.text = "<i>No Traits</i>";
            }
        }

        if (selectionToggle != null)
        {
            selectionToggle.onValueChanged.AddListener(OnToggleChanged);
        }
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
