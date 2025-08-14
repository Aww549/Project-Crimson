using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Visual Settings")]
    public GameObject survivorPrefab;
    public GameObject muzzleFlashPrefab;
    public int maxVisualSurvivors = 20;

    [Header("Formation Settings")]
    public int survivorsPerRow = 5;
    public float rowSpacing = 2.0f;
    public float positionSpacing = 1.5f;

    private List<GameObject> visualSurvivors = new List<GameObject>();

    public void AddVisualSurvivor()
    {
        if (visualSurvivors.Count >= maxVisualSurvivors)
        {
            return;
        }

        int currentCount = visualSurvivors.Count;
        int rowIndex = currentCount / survivorsPerRow;
        int columnIndex = currentCount % survivorsPerRow;

        // Simple grid formation behind the player.
        float xPos = (columnIndex - (survivorsPerRow - 1) * 0.5f) * positionSpacing;
        float zPos = -(rowIndex + 1) * rowSpacing;

        Vector3 spawnPos = new Vector3(xPos, 0, zPos);

        // Spawn the survivor as a child of this object.
        GameObject newSurvivor = Instantiate(survivorPrefab, transform);
        newSurvivor.transform.localPosition = spawnPos;

        visualSurvivors.Add(newSurvivor);
    }

    public void TriggerMuzzleFlash()
    {
        if (visualSurvivors.Count == 0) return;

        int randomIndex = Random.Range(0, visualSurvivors.Count);
        GameObject randomSurvivor = visualSurvivors[randomIndex];
        Transform firePoint = randomSurvivor.transform.Find("FirePoint");
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(randomSurvivor.transform);
            firePoint = fp.transform;
        }
        firePoint.transform.localPosition = new Vector3(0, 1.5f, 1f);

        if (muzzleFlashPrefab != null)
        {
            StartCoroutine(FlashEffect(firePoint));
        }
    }

    private IEnumerator FlashEffect(Transform spawnPoint)
    {
        GameObject flash = Instantiate(muzzleFlashPrefab, spawnPoint.position, spawnPoint.rotation);
        yield return new WaitForSeconds(0.08f);
        Destroy(flash);
    }
}
