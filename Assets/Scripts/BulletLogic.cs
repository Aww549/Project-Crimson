using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 30f;
    public float lifetime = 2f;
    public float damage;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * moveSpeed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // --- UNIVERSAL ZOMBIE CHECK ---
        // First, check if the object we hit is a normal zombie.
        ZombieLogic normalZombie = other.GetComponent<ZombieLogic>();
        if (normalZombie != null)
        {
            normalZombie.TakeDamage(damage);
            Destroy(gameObject);
            return; // Exit after dealing damage.
        }

        // If it's not a normal zombie, check if it's a Runner.
        RunnerZombieLogic runnerZombie = other.GetComponent<RunnerZombieLogic>();
        if (runnerZombie != null)
        {
            runnerZombie.TakeDamage(damage);
            Destroy(gameObject);
            return; // Exit after dealing damage.
        }

        // If it's not any type of zombie, check if it's a gate.
        Gate gate = other.GetComponent<Gate>();
        if (gate != null)
        {
            gate.TakeShot();
            Destroy(gameObject);
        }
    }
}
