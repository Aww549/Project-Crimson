using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SurvivorListItemUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    [SerializeField] private GameObject frontPanel;
    [SerializeField] private GameObject backPanel;
    [SerializeField] private GameObject onMissionOverlay;

    [Header("Front Panel")]
    [SerializeField] private TextMeshProUGUI survivorNameText;
    [SerializeField] private TextMeshProUGUI missionTimerText;

    [Header("Back Panel")]
    [SerializeField] private TextMeshProUGUI back_survivorNameText;
    [SerializeField] private TextMeshProUGUI back_traitsText;

    private Survivor assignedSurvivor;
    private bool isFlipped = false;

    private Coroutine timerRoutine;

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

    public void Setup(Survivor survivor)
    {
        assignedSurvivor = survivor;

        if (survivorNameText != null)
        {
            survivorNameText.text = survivor.survivorName;
            Debug.Log($"Setting up Survivor UI for: {survivor.survivorName} ");
            Debug.Log($"  - Set survivorNameText to: {survivor.survivorName} ");
        }

        if (back_survivorNameText != null)
        {
            back_survivorNameText.text = survivor.survivorName;
            Debug.Log($"  - Set back_survivorNameText to: {survivor.survivorName}");
        }

        if (back_traitsText != null)
        {
            if (survivor.traits == null || survivor.traits.Count == 0)
            {
                back_traitsText.text = "<i>No Traits</i>";
                Debug.Log("  - Survivor has no traits.");
            }
            else
            {
                StringBuilder traitsBuilder = new StringBuilder();
                foreach (var trait in survivor.traits.Where(t => t != null))
                {
                    traitsBuilder.AppendLine($"<b>{trait.traitName}</b>: {trait.description}");
                }
                back_traitsText.text = traitsBuilder.ToString();
                Debug.Log($"  - Set traits text to: {traitsBuilder.ToString().Trim()}");
            }
        }

        UpdateStatusView();
    }

    public void UpdateStatusView()
    {
        if (assignedSurvivor == null) return;

        if (assignedSurvivor.status == SurvivorStatus.OnMission)
        {
            if (frontPanel != null) frontPanel.SetActive(true);
            if (backPanel != null) backPanel.SetActive(false);
            if (onMissionOverlay != null) onMissionOverlay.SetActive(true);

            StartTimer();
        }
        else
        {
            if (onMissionOverlay != null) onMissionOverlay.SetActive(false);

            // Show front/back based on flip state when Idle/Wounded
            if (frontPanel != null) frontPanel.SetActive(!isFlipped);
            if (backPanel != null) backPanel.SetActive(isFlipped);

            StopTimer();
            if (missionTimerText != null) missionTimerText.text = string.Empty;
        }
    }

    private void StartTimer()
    {
        StopTimer();
        timerRoutine = StartCoroutine(TimerCoroutine());
    }

    private void StopTimer()
    {
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
    }

    private System.Collections.IEnumerator TimerCoroutine()
    {
        while (true)
        {
            if (assignedSurvivor == null || GameDataManager.Instance == null || GameDataManager.Instance.gameData == null)
            {
                if (missionTimerText != null) missionTimerText.text = string.Empty;
                yield break;
            }

            var mission = GameDataManager.Instance.gameData.activeMissions
                .FirstOrDefault(m => m.missionId == assignedSurvivor.assignedMissionId);

            if (mission == null)
            {
                if (missionTimerText != null) missionTimerText.text = string.Empty;
                yield break;
            }

            if (mission.isReadyToClaim)
            {
                if (missionTimerText != null) missionTimerText.text = "Complete";
            }
            else
            {
                long remainingTicks = mission.missionEndTimeTicks - DateTime.UtcNow.Ticks;
                if (remainingTicks < 0) remainingTicks = 0;
                var ts = TimeSpan.FromTicks(remainingTicks);
                if (missionTimerText != null)
                {
                    missionTimerText.text = $"{(int)ts.TotalMinutes:00}:{ts.Seconds:00}";
                }
            }

            // Update once per second
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    // Allow clicking the card to flip when not on a mission.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (assignedSurvivor == null) return;

        // Do not flip while on mission (overlay visible)
        if (assignedSurvivor.status == SurvivorStatus.OnMission) return;

        isFlipped = !isFlipped;
        UpdateStatusView();
    }

    void OnDisable()
    {
        StopTimer();
    }

    void OnDestroy()
    {
        StopTimer();
    }
}