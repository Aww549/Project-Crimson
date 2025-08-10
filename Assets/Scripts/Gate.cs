using UnityEngine;

public enum GateType { AddSurvivors, AddDamage }

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public GateType type;
    public int value;
    public int shotsToActivate = 3;

    private int currentShots = 0;

    // This is now the ONLY way to interact with the gate.
    public void TakeShot()
    {
        currentShots++;

        // Add a visual "crack" or "hit" effect here later

        if (currentShots >= shotsToActivate)
        {
            // The gate has taken enough shots, activate the power-up and destroy it.
            ActivateAndDestroy();
        }
    }

    private void ActivateAndDestroy()
    {
        PlayerManager player = FindFirstObjectByType<PlayerManager>();
        if (player != null)
        {
            if (type == GateType.AddSurvivors)
            {
                player.AddSurvivors(value);
            }
            else if (type == GateType.AddDamage)
            {
                player.AddDamage((float)value);
            }
        }

        // Add a satisfying "shatter" or "power-up" effect here later
        Debug.Log(type.ToString() + " GATE ACTIVATED!");
        Destroy(gameObject);
    }
}