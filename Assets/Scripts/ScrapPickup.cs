using UnityEngine;

/// <summary>
/// Handles the logic for when a player collects a scrap item.
/// </summary>
public class ScrapPickup : MonoBehaviour
{
    // The base value of a scrap pickup.
    public int baseScrapValue = 1;

    void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            int finalScrapValue = baseScrapValue;

            // --- APPLY PERMANENT UPGRADE ---
            // Check the "Scrap Scavenging" upgrade level to determine the final value.
            if (GameDataManager.Instance != null)
            {
                // The final value is the base value plus the upgrade level.
                // Level 1 = +0, Level 2 = +1, etc.
                finalScrapValue += (GameDataManager.Instance.ScrapValueUpgradeLevel - 1);
            }

            player.AddScrap(finalScrapValue);
            Destroy(gameObject);
        }
    }
}
