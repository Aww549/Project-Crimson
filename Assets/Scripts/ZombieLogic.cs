using UnityEngine;

/// <summary>
/// Controls the behavior of a single zombie, including its health, activation, and movement.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ZombieLogic : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float moveSpeed = 1f;

    [Header("Activation")]
    public float activationDistance = 30f;

    // --- Private Fields ---
    private Transform playerTransform;
    private Rigidbody rb;
    private Renderer objectRenderer; // A reference to the zombie's renderer to change its color.
    private bool isActivated = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Get the renderer component. It might be on a child object, so we use GetComponentInChildren.
        objectRenderer = GetComponentInChildren<Renderer>();

        if (rb == null)
        {
            Debug.LogError("ZombieLogic Error: Rigidbody component not found on " + gameObject.name);
        }

        PlayerManager player = FindFirstObjectByType<PlayerManager>();
        if (player != null)
        {
            playerTransform = player.transform;
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

    /// <summary>
    /// Turns this zombie into an Elite by changing its color.
    /// </summary>
    /// <param name="eliteColor">The new color for the zombie's material.</param>
    public void MakeElite(Color eliteColor)
    {
        if (objectRenderer != null)
        {
            // We change the color of the material instance for this specific zombie.
            objectRenderer.material.color = eliteColor;
        }
        else
        {
            Debug.LogWarning("Cannot apply Elite color: No Renderer found on " + gameObject.name);
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
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerManager>() != null)
        {
            FindFirstObjectByType<UIManager>().GameOver();
            Destroy(other.gameObject);
        }
    }
}
