using UnityEngine;

public class CleanupCrewLogic : MonoBehaviour
{
    public Transform playerTransform;
    public float distanceBehindPlayer = 10f;

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, 0, playerTransform.position.z - distanceBehindPlayer);
            transform.position = targetPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Survivor"))
        {
            return;
        }

        // Check if the object that hit is a zombie (of any type)
        if (other.GetComponent<ZombieLogic>() != null || other.GetComponent<RunnerZombieLogic>() != null)
        {
            // *** NEW: Check if the player has a shield. ***
            if (PlayerManager.Instance != null && PlayerManager.Instance.shieldCount > 0)
            {
                // If they do, consume the shield and destroy the zombie. The run continues!
                PlayerManager.Instance.UseShield();
                Destroy(other.gameObject);
            }
            else
            {
                // If there's no shield, it's game over as normal.
                PlayerManager player = FindFirstObjectByType<PlayerManager>();
                if (player != null)
                {
                    FindFirstObjectByType<UIManager>().GameOver();
                    Destroy(player.gameObject);
                }
                Destroy(other.gameObject);
            }
            return;
        }

        // The rest of the cleanup logic for other objects.
        int objectLayer = other.gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        int playerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
        int environmentLayer = LayerMask.NameToLayer("Environment");

        if (objectLayer != playerLayer && objectLayer != playerBulletLayer && objectLayer != environmentLayer)
        {
            Destroy(other.gameObject);
        }
    }
}
