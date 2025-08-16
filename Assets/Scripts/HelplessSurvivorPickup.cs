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
            if (GameDataManager.Instance != null)
            {
                // Tell the GameDataManager which archetype was rescued.
                GameDataManager.Instance.SetRescuedSurvivor(rescuedArchetype);

                Debug.Log($"Player picked up survivor: {rescuedArchetype.archetypeName}");
            }
            Destroy(gameObject);
        }
    }
}
