using UnityEngine;
using System.Collections.Generic;

public class SanctuaryManager : MonoBehaviour
{
    public static SanctuaryManager Instance { get; private set; }

    // This manager no longer needs to know about individual traits,
    // as that is all handled by the Archetype assets now.

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Creates a new survivor based on a chosen Archetype, assigning its name and traits.
    /// </summary>
    /// <param name="archetype">The SurvivorArchetype asset to create the survivor from.</param>
    public void CreateAndAddSurvivor(SurvivorArchetype archetype)
    {
        if (archetype == null)
        {
            Debug.LogError("CreateAndAddSurvivor was called with a null archetype!");
            return;
        }

        Survivor newSurvivor = new Survivor();

        // Assign the name and traits directly from the archetype
        newSurvivor.survivorName = archetype.archetypeName;
        newSurvivor.traits = new List<Trait>(archetype.traits); // Create a new list copy

        GameDataManager.Instance.gameData.sanctuarySurvivors.Add(newSurvivor);
        GameDataManager.Instance.SaveGame();
        Debug.Log($"Added new '{newSurvivor.survivorName}' survivor to Sanctuary.");
    }
}
