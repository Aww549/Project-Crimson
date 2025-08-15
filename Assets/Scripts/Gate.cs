using UnityEngine;

public enum GateType { AddSurvivors, AddDamage }

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public GateType type;
    public int value;
    public int shotsToActivate = 3;

    private int currentShots = 0;

    public void TakeShot()
    {
        currentShots++;
        if (currentShots >= shotsToActivate)
        {
            ActivateAndDestroy();
        }
    }

    private void ActivateAndDestroy()
    {
        // *** FIX: Use the reliable static instance instead of searching. ***
        if (PlayerManager.Instance != null)
        {
            if (type == GateType.AddSurvivors)
            {
                PlayerManager.Instance.AddSurvivors(value);
            }
            else if (type == GateType.AddDamage)
            {
                PlayerManager.Instance.AddDamage((float)value);
            }
        }
        else
        {
            Debug.LogError("Gate Error: Could not find PlayerManager.Instance!");
        }

        Destroy(gameObject);
    }
}
