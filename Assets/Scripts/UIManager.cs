using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all UI elements in the scene, including in-game stats and game state screens.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("In-Game UI")]
    public TextMeshProUGUI survivorCountText;
    public TextMeshProUGUI scrapCountText;

    [Header("Game State UI")]
    public GameObject gameOverUI;
    public GameObject restartButton; // A separate, direct reference to the restart button.

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Ensure the main game over screen is hidden at the start of the run.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // For testing purposes, we want the restart button to be visible at all times.
        // We enable it here directly.
        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }
    }

    /// <summary>
    /// Activates the Game Over UI elements.
    /// </summary>
    public void GameOver()
    {
        // The restart button is already visible, so we only need to show the rest of the Game Over UI.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    /// <summary>
    /// Reloads the current scene. This function should be linked to the RestartButton's OnClick() event.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Updates the survivor count text on the screen.
    /// </summary>
    /// <param name="newCount">The new survivor count.</param>
    public void UpdateSurvivorCount(int newCount)
    {
        if (survivorCountText != null)
        {
            survivorCountText.text = "Survivors: " + newCount;
        }
    }

    /// <summary>
    /// Updates the scrap count text on the screen.
    /// </summary>
    /// <param name="newCount">The new scrap count.</param>
    public void UpdateScrapCount(int newCount)
    {
        if (scrapCountText != null)
        {
            scrapCountText.text = "Scrap: " + newCount;
        }
    }
}
