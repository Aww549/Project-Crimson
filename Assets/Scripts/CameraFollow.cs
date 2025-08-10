using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // A reference to the object we want to follow (the Player)
    public Vector3 offset;   // The distance and angle from the player

    // LateUpdate is called once per frame, AFTER all other Update functions have finished.
    // This is the best place for camera logic, as it ensures the player
    // has already moved for this frame before the camera updates its position.
    // This prevents a common visual jitter effect.
    void LateUpdate()
    {
        // Check if a target has been assigned so we don't get an error
        if (target != null)
        {
            // Set the camera's position to be the target's position plus the offset
            transform.position = target.position + offset;
        }
    }
}