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
            // DEFINITIVE FIX: Add a check to ensure we only ever pick up ONE survivor per run.
            // This prevents the data from being overwritten if the spawner bug were to happen again.
            if (GameDataManager.Instance != null && GameDataManager.Instance.gameData.survivorsRescuedThisRun == 0)
            {
                // Tell the GameDataManager which archetype was rescued.
                GameDataManager.Instance.SetRescuedSurvivor(rescuedArchetype);

                Debug.Log($"Player picked up survivor: {rescuedArchetype.archetypeName}");

                Destroy(gameObject);
            }
            // If a survivor has already been rescued (survivorsRescuedThisRun > 0), this pickup
            // will simply do nothing, protecting the integrity of the first pickup.
        }
    }
}
