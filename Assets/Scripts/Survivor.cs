using System;
using System.Collections.Generic;

[System.Serializable]
public class Survivor
{
    public string survivorId;
    public string survivorName;
    public SurvivorStatus status;
    public string assignedMissionId;
    public List<Trait> traits;

    public Survivor()
    {
        survivorId = Guid.NewGuid().ToString();
        status = SurvivorStatus.Idle;

        // --- FIX ---
        // The name is no longer set here. It will be assigned by the
        // SanctuaryManager when the survivor is created from an archetype.
        survivorName = "Unnamed";

        assignedMissionId = string.Empty;
        traits = new List<Trait>();
    }
}

public enum SurvivorStatus { Idle, OnMission, Wounded }
