using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ZombieLogic : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float moveSpeed = 1f;

    [Header("Activation")]
    public float activationDistance = 30f;

    private Transform playerTransform;
    private Rigidbody rb;
    private Renderer objectRenderer;
    private bool isActivated = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectRenderer = GetComponentInChildren<Renderer>();

        if (rb == null)
        {
            Debug.LogError("ZombieLogic Error: Rigidbody component not found on " + gameObject.name);
        }

        // Use the static instance for reliability
        if (PlayerManager.Instance != null)
        {
            playerTransform = PlayerManager.Instance.transform;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (!isActivated && playerTransform != null)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) < activationDistance)
            {
                isActivated = true;
            }
        }

        if (isActivated)
        {
            rb.linearVelocity = Vector3.back * moveSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if we hit the player
        if (other.GetComponent<PlayerManager>() != null)
        {
            // *** NEW: Check if the player has a shield. ***
            if (PlayerManager.Instance.shieldCount > 0)
            {
                // If they do, use the shield and the zombie dies.
                PlayerManager.Instance.UseShield();
                Die(); // The zombie sacrifices itself to break the shield.
            }
            else
            {
                // If there's no shield, it's game over.
                FindFirstObjectByType<UIManager>().GameOver();
                Destroy(other.gameObject);
            }
        }
    }

    public void MakeElite(Color eliteColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = eliteColor;
        }
    }

    public void SetInitialHealth(float amount)
    {
        health = amount;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.zombiesKilled++;
        }
        Destroy(gameObject);
    }
}
