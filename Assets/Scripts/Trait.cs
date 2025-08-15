using System;
using UnityEngine; // Required for CreateAssetMenu

/// <summary>
/// A ScriptableObject that holds all the static data for a single survivor trait.
/// This allows us to create and balance different traits as assets in the Unity Editor.
/// </summary>
[CreateAssetMenu(fileName = "NewTrait", menuName = "Dead Ahead/Trait Data", order = 2)]
public class Trait : ScriptableObject // Changed from a simple class to a ScriptableObject
{
    /// <summary>
    /// The display name of the trait (e.g., "Scout", "Clumsy").
    /// </summary>
    public string traitName;

    /// <summary>
    /// A short description of the trait's effect for UI tooltips.
    /// </summary>
    [TextArea(2, 4)]
    public string description;

    // --- MODIFIERS ---
    // Modifiers are represented as floats. 
    // E.g., 0.10 for a +10% bonus, -0.05 for a -5% penalty.
    // A value of 0 means the trait has no effect on that particular stat.

    /// <summary>
    /// Modifies the mission's success chance.
    /// </summary>
    public float successChanceModifier;

    /// <summary>
    /// Modifies the mission's reward yield (Scrap or Materials).
    /// </summary>
    public float rewardModifier;

    /// <summary>
    /// Modifies the mission's duration. A negative value reduces time, a positive value increases it.
    /// </summary>
    public float durationModifier;
}
