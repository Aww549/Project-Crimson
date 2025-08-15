using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the rarity categories for Survivor Archetypes.
/// </summary>
public enum SurvivorRarity { Common, Uncommon, Legendary }

/// <summary>
/// A ScriptableObject that defines a pre-designed Survivor Archetype.
/// This includes their name, rarity, and a hand-picked list of traits.
/// </summary>
[CreateAssetMenu(fileName = "NewSurvivorArchetype", menuName = "Dead Ahead/Survivor Archetype", order = 3)]
public class SurvivorArchetype : ScriptableObject
{
    [Header("Archetype Details")]
    public string archetypeName;
    [TextArea(2, 4)]
    public string description;
    public SurvivorRarity rarity;

    [Header("Traits")]
    [Tooltip("The specific list of Trait assets that define this archetype.")]
    public List<Trait> traits;
}
