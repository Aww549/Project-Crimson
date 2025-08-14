using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the UI and functionality of the main menu scene.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "SampleScene";
    public string campSceneName = "SurvivorCamp";

    /// <summary>
    /// Loads the main gameplay scene to start a new run.
    /// This should be linked to the "Start Run" button.
    /// </summary>
    public void StartRun()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Loads the Survivor Camp scene for upgrades.
    /// This should be linked to the "Survivor Camp" button.
    /// </summary>
    public void GoToCamp()
    {
        SceneManager.LoadScene(campSceneName);
    }
}
