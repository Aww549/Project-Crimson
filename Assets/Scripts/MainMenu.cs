// MainMenu.cs (Updated)
// This is the clean, stable logic script for the main menu.
// Its only job is to handle button clicks and load scenes.

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("--- Scene Configuration ---")]
    [Tooltip("The exact name of the main gameplay scene file (e.g., SampleScene).")]
    [SerializeField] private string mainGameSceneName = "SampleScene";

    [Tooltip("The exact name of the Survivor Camp scene file.")]
    [SerializeField] private string survivorCampSceneName = "SurvivorCamp";

    // **THE FIX IS HERE (Part 1)**
    // This variable will hold a reference to the entire UI canvas.
    [HideInInspector] // Hides this from the Inspector to prevent confusion
    public GameObject mainMenuCanvasObject;


    /// <summary>
    /// Loads the main gameplay scene.
    /// Called by the 'START RUN' button.
    /// </summary>
    public void StartRun()
    {
        if (mainMenuCanvasObject != null) mainMenuCanvasObject.SetActive(false);
        SceneManager.LoadScene(mainGameSceneName);
    }

    /// <summary>
    /// Loads the Survivor Camp scene.
    /// Called by the 'SURVIVOR CAMP' button.
    /// </summary>
    public void GoToSurvivorCamp()
    {
        // **THE FIX IS HERE (Part 2)**
        // We now disable the canvas before loading the next scene.
        if (mainMenuCanvasObject != null)
        {
            mainMenuCanvasObject.SetActive(false);
        }
        SceneManager.LoadScene(survivorCampSceneName);
    }

    /// <summary>
    /// Exits the application.
    /// Called by the 'EXIT' button.
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Exit button clicked. Application will quit in a built game.");
        Application.Quit();
    }
}
