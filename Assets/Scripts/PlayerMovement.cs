using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    public float moveSpeed = 5f;
    public float laneChangeSpeed = 15f;

    [Header("Lane Info")]
    public float laneDistance = 6f;
    private int targetLane = 0; // 0=Left, 1=Right

    void Update()
    {
        // Always move forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // --- LANE SWITCHING ---
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && targetLane == 1)
        {
            targetLane = 0;
        }
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && targetLane == 0)
        {
            targetLane = 1;
        }

        Vector3 targetPosition = transform.position;
        targetPosition.x = (targetLane - 0.5f) * laneDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneChangeSpeed * Time.deltaTime);
    }
}
