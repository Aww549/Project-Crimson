using UnityEngine;

public class BulletLogic : MonoBehaviour
{
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
        // Check for a Gate
        Gate gate = other.GetComponent<Gate>();
        if (gate != null)
        {
            gate.TakeShot();
            Destroy(gameObject); // Destroy bullet after hitting a gate
            return;
        }

        // Check for a Zombie
        ZombieLogic zombie = other.GetComponent<ZombieLogic>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            Destroy(gameObject); // Destroy bullet after hitting a zombie
        }
    }
}