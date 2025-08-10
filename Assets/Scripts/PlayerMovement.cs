using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    public float moveSpeed = 5f;
    public float laneChangeSpeed = 15f;

    [Header("Lane Info")]
    public float laneDistance = 3f; // Let's default to a wider distance
    private int targetLane = 0; // The lane we want to be in (0=Left, 1=Right)

    void Update()
    {
        // --- FORWARD MOVEMENT ---
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // --- INPUT & LANE SWITCHING ---
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetLane--;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetLane++;
        }

        // We now clamp the value between 0 and 1 for our two lanes
        targetLane = Mathf.Clamp(targetLane, 0, 1);

        // --- CALCULATE TARGET POSITION ---
        // A new calculation for two lanes centered around the world's zero axis
        // If targetLane is 0, x = -1.5. If targetLane is 1, x = 1.5 (with default distance)
        Vector3 targetPosition = transform.position;
        targetPosition.x = (targetLane - 0.5f) * laneDistance;

        // --- SMOOTH MOVEMENT TO TARGET ---
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneChangeSpeed * Time.deltaTime);
    }
}