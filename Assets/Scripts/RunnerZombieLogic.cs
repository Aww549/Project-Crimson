using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RunnerZombieLogic : MonoBehaviour
{
    [Header("Stats")]
    public float health = 75f;
    public float moveSpeed = 4f;

    private Rigidbody rb;
    private Renderer objectRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectRenderer = GetComponentInChildren<Renderer>();

        if (rb == null)
        {
            Debug.LogError("RunnerZombieLogic Error: Rigidbody component not found on " + gameObject.name);
        }
    }

    void Start()
    {
        if (rb != null)
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
                Die();
            }
            else
            {
                // If there's no shield, it's game over.
                FindFirstObjectByType<UIManager>().GameOver();
                Destroy(other.gameObject);
            }
        }
    }

    public void SetInitialHealth(float amount)
    {
        health = amount;
    }

    public void MakeElite(Color eliteColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = eliteColor;
        }
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
}
