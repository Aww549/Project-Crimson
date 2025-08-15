using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform playerTransform;

    // This is now called manually by the CameraManager.
    public void ManualUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(0, transform.position.y, playerTransform.position.z);
        }
    }
}
