using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
    public int baseScrapValue = 1;

    void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            int finalScrapValue = baseScrapValue;

            if (GameDataManager.Instance != null)
            {
                // *** FIX: Accessing data through the 'gameData' object ***
                finalScrapValue += (GameDataManager.Instance.gameData.scrapValueUpgradeLevel - 1);
            }

            player.AddScrap(finalScrapValue);
            Destroy(gameObject);
        }
    }
}
