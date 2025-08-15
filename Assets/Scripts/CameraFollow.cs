using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform cameraTarget;
    public PlayerManager playerManager;

    [Header("Camera Settings")]
    public float followSmoothSpeed = 0.125f;
    public float zoomSmoothSpeed = 2.0f;

    [Header("Dynamic Offset")]
    public Vector3 minOffset = new Vector3(0, 5, -8);
    public Vector3 maxOffset = new Vector3(0, 10, -15);
    public int survivorCountForMaxOffset = 20;

    private float currentZoomT = 0f;

    // This is now called manually by the CameraManager.
    public void ManualUpdate()
    {
        if (cameraTarget == null || playerManager == null)
        {
            return;
        }

        float targetZoomT = (float)playerManager.survivorCount / survivorCountForMaxOffset;
        targetZoomT = Mathf.Clamp01(targetZoomT);

        currentZoomT = Mathf.Lerp(currentZoomT, targetZoomT, Time.deltaTime * zoomSmoothSpeed);

        Vector3 desiredOffset = Vector3.Lerp(minOffset, maxOffset, currentZoomT);
        Vector3 desiredPosition = cameraTarget.position + desiredOffset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSmoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(cameraTarget);
    }
}
