using UnityEngine;

public class HelplessSurvivorPickup : MonoBehaviour
{
    private SurvivorArchetype rescuedArchetype;

    /// <summary>
    /// Called by the Spawner to assign which archetype this pickup represents.
    /// </summary>
    public void SetArchetype(SurvivorArchetype archetype)
    {
        rescuedArchetype = archetype;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ensure it's the player and that an archetype has been assigned.
        if (rescuedArchetype != null && other.GetComponent<PlayerManager>() != null)
        {
            // We should only be able to pick up one survivor per run.
            if (GameDataManager.Instance != null && GameDataManager.Instance.gameData.survivorsRescuedThisRun == 0)
            {
                // Create a new survivor from the archetype
                Survivor newSurvivor = new Survivor
                {
                    survivorName = rescuedArchetype.archetypeName,
                    traits = new List<Trait>(rescuedArchetype.traits)
                };

                // Add the new survivor to the sanctuary roster
                GameDataManager.Instance.AddSurvivor(newSurvivor);

                // Also call the original method to track rescued count
                GameDataManager.Instance.SetRescuedSurvivor(rescuedArchetype);

                Debug.Log($"Player rescued survivor: {newSurvivor.survivorName}. Added to sanctuary.");

                Destroy(gameObject);
            }
        }
    }
}
