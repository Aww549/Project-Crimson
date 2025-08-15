using UnityEngine;

/// <summary>
/// A central manager that controls the update order for all camera components
/// to ensure a smooth, jitter-free follow.
/// </summary>
public class CameraManager : MonoBehaviour
{
    [Header("References")]
    public CameraTarget cameraTarget;
    public CameraFollow cameraFollow;

    // We use LateUpdate to ensure the player has finished all movement for the frame.
    void LateUpdate()
    {
        // First, update the stable target's position.
        cameraTarget.ManualUpdate();

        // Second, update the main camera's position based on the now-stable target.
        cameraFollow.ManualUpdate();
    }
}
