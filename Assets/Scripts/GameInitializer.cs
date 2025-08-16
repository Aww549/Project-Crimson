// GameInitializer.cs
// This script lives in the Initialization scene (build index 0).
// Its only purpose is to load the MainMenu after the persistent managers
// in this scene have been created.

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // This script assumes that GameDataManager, MissionController, etc.
        // are in the same scene and will be created before this Start() method is called.

        // Immediately load the main menu scene to start the game.
        SceneManager.LoadScene("MainMenu");
    }
}
