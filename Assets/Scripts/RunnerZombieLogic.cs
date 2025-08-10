using UnityEngine;

/// <summary>
/// Controls the Runner Zombie, which moves towards the player immediately upon spawning.
/// It inherits all logic from ZombieLogic and simply forces immediate activation
/// by setting the activation distance to a very large number.
/// </summary>
public class RunnerZombieLogic : ZombieLogic
{
    void Start()
    {
        // By setting the activation distance to a huge value, this zombie will
        // consider itself "activated" as soon as it spawns, causing it to move immediately.
        // This is a simple and non-invasive way to change the behavior without
        // needing to rewrite the base class's movement logic.
        activationDistance = 1000f;
    }
}
