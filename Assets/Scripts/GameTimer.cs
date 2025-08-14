using UnityEngine;

/// <summary>
/// Manages the in-run timer. When the timer reaches zero, the run is considered complete.
/// </summary>
public class GameTimer : MonoBehaviour
{
    [Header("Settings")]
    public float runDuration = 60f; // The total length of a run in seconds.

    [Header("References")]
    public UIManager uiManager;

    private float timeLeft;
    private bool isTimerRunning = false;

    void Start()
    {
        timeLeft = runDuration;
        isTimerRunning = true;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                if (uiManager != null)
                {
                    uiManager.UpdateTimerDisplay(timeLeft);
                }
            }
            else
            {
                timeLeft = 0;
                isTimerRunning = false;
                if (uiManager != null)
                {
                    uiManager.UpdateTimerDisplay(timeLeft);
                }
                EndRun();
            }
        }
    }

    /// <summary>
    /// Called when the timer reaches zero.
    /// </summary>
    private void EndRun()
    {
        Debug.Log("TIMER FINISHED! Run complete.");

        // *** THE FIX: Explicitly call the GameOver function on the UIManager. ***
        // This will show the end-of-run screen and pause the game.
        if (uiManager != null)
        {
            uiManager.GameOver();
        }
    }
}
