using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("In-Game UI")]
    public TextMeshProUGUI survivorCountText;
    public TextMeshProUGUI scrapCountText;
    // *** NEW: A reference for the timer display. ***
    public TextMeshProUGUI timerText;

    [Header("Game State UI")]
    public GameObject gameOverPanel;

    [Header("Scene Management")]
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        Time.timeScale = 1f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// *** NEW: Updates the timer text on the screen, formatted as Minutes:Seconds. ***
    /// </summary>
    public void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null) return;

        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        // Format the time into minutes and seconds for the display
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateSurvivorCount(int newCount)
    {
        if (survivorCountText != null)
        {
            survivorCountText.text = "Survivors: " + newCount;
        }
    }

    public void UpdateScrapCount(int newCount)
    {
        if (scrapCountText != null)
        {
            scrapCountText.text = "Scrap: " + newCount;
        }
    }
}
