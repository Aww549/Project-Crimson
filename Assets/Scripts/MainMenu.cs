using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel; // The parent panel for the main menu buttons

    [Header("Scene Names")]
    public string gameSceneName = "SampleScene";
    public string campSceneName = "SurvivorCamp";

    public void StartRun()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToCamp()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }
        SceneManager.LoadScene(campSceneName);
    }
}
