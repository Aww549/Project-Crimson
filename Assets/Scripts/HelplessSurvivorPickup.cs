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
            // This check ensures we only ever "rescue" ONE survivor per run.
            // This prevents the data from being overwritten if the spawner were to create multiple.
            if (GameDataManager.Instance != null && GameDataManager.Instance.gameData.survivorsRescuedThisRun == 0)
            {
                // Tell the GameDataManager which archetype was rescued.
                // The actual Survivor object is created at the end of the run by the SanctuaryManager.
                GameDataManager.Instance.SetRescuedSurvivor(rescuedArchetype);

                Debug.Log($"Player picked up survivor: {rescuedArchetype.archetypeName}. This will be processed at the end of the run.");

                Destroy(gameObject);
            }
        }
    }
}
