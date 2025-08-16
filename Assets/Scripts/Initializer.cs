// Initializer.cs (Final Version)
// This script ensures that your core game managers always exist.
// The check has been moved to Start() to prevent race conditions during scene loads.

using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    // **THE FIX IS HERE**
    // The logic is now in Start() instead of Awake().
    void Start()
    {
        // Check if the core managers have been created.
        if (GameDataManager.Instance == null || MissionController.Instance == null)
        {
            // If they haven't, it means we started from the wrong scene
            // or they were destroyed during a scene transition.
            // Load the MainMenu scene to create them.
            Debug.LogWarning("Core managers not found. Loading MainMenu scene to initialize.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
