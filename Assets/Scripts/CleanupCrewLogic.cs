using UnityEngine;

public class CleanupCrewLogic : MonoBehaviour
{
    public Transform playerTransform;
    public float distanceBehindPlayer = 10f;

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(playerTransform.position.x, 0, playerTransform.position.z - distanceBehindPlayer);
        transform.position = targetPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        // --- FIRST PRIORITY: Check if a zombie got past ---
        // We try to get the ZombieLogic component from the object that hit us.
        if (other.GetComponent<ZombieLogic>() != null)
        {
            PlayerManager player = FindFirstObjectByType<PlayerManager>();
            if (player != null)
            {
                Debug.Log("A zombie got past! GAME OVER.");
                Destroy(player.gameObject);
            }
            // Destroy the zombie that got past
            Destroy(other.gameObject);
            return; // We exit the function early since we've handled this object.
        }

        // --- SECOND PRIORITY: If it wasn't a zombie, run our cleanup logic ---
        int objectLayer = other.gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        int playerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
        int environmentLayer = LayerMask.NameToLayer("Environment");

        // If the object's layer is NOT the player, AND NOT a bullet, AND NOT the environment...
        if (objectLayer != playerLayer && objectLayer != playerBulletLayer && objectLayer != environmentLayer)
        {
            // ...then it must be an old gate or other debris, so it's safe to destroy.
            Destroy(other.gameObject);
        }
    }
}