using UnityEngine;

/// <summary>
/// Enum to define the primary reward type for a mission.
/// </summary>
public enum MissionRewardType { Materials, Scrap }

/// <summary>
/// A ScriptableObject that holds all the static data for a mission template.
/// This allows us to create and balance different missions as assets in the Unity Editor.
/// </summary>
[CreateAssetMenu(fileName = "NewMission", menuName = "Dead Ahead/Mission Data", order = 1)]
public class MissionData : ScriptableObject
{
    [Header("Mission Details")]
    public string missionName;
    [TextArea(3, 5)]
    public string description;
    public MissionRewardType rewardType;

    [Header("Mission Parameters")]
    public float durationHours; // Mission duration in real-world hours.
    [Range(0, 1)]
    public float baseSuccessChance; // The success chance with only one survivor assigned.
    public int baseRewardAmount;

    [Header("Survivor Slots")]
    [Range(1, 3)]
    public int maxSurvivors = 3; // The maximum number of survivors that can be assigned.

    // Bonus success chance for each additional survivor after the first.
    public float bonusSuccessChancePerSurvivor = 0.2f; // e.g., 20% bonus for 2nd survivor.
}
