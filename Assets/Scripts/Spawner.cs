using UnityEngine;
using System.Collections.Generic;

public enum WaveType { Gauntlet, Loot, EliteStandoff, RunnerWave }

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public GameObject zombiePrefab;
    public GameObject runnerZombiePrefab;
    public GameObject[] gatePrefabs;
    public GameObject scrapPrefab;
    public Transform playerTransform;

    [Header("Spawner Settings")]
    public float spawnDistance = 70f;
    public float distanceBetweenRows = 15f;
    public float laneDistance = 6f;
    public float zombieSpacing = 4.5f;

    [Header("Wave Type Probability")]
    [Range(0, 1)]
    public float lootWaveChance = 0.25f;
    [Range(0, 1)]
    public float runnerWaveChance = 0.15f;

    [Header("Difficulty Scaling")]
    private int waveNumber = 0;
    public int startZombies = 1;
    public int maxZombies = 5;
    public float baseZombieHealth = 60f;
    public float healthPerWave = 1.5f;
    // *** BALANCE CHANGE: Added a max health cap for normal zombies. ***
    public float maxNormalZombieHealth = 200f;

    [Header("Elite Zombie Settings")]
    public int minWaveForElites = 10;
    public Color eliteColor = Color.magenta;

    // *** NEW: Counter for the "Breather Wave" logic. ***
    private int consecutiveToughWaves = 0;

    private float nextSpawnZ;
    private WaveType lastWaveType;
    private int lastThreatLane = -1;

    void Start()
    {
        nextSpawnZ = transform.position.z;
        lastWaveType = WaveType.Gauntlet;
        for (int i = 0; i < 3; i++)
        {
            SpawnRow();
        }
    }

    void Update()
    {
        if (playerTransform.position.z > nextSpawnZ - spawnDistance)
        {
            SpawnRow();
        }
    }

    void SpawnRow()
    {
        waveNumber++;
        float roll = Random.value;

        // --- BREATHER WAVE LOGIC ---
        // If we've had 3 tough waves in a row, force a Loot Wave.
        if (consecutiveToughWaves >= 3)
        {
            SpawnLootWave();
            lastWaveType = WaveType.Loot;
            consecutiveToughWaves = 0; // Reset the counter
        }
        else if (waveNumber >= minWaveForElites && roll < 0.33f)
        {
            SpawnEliteStandoffWave();
            lastWaveType = WaveType.EliteStandoff;
            consecutiveToughWaves++;
        }
        else if (waveNumber > 3 && roll < runnerWaveChance)
        {
            SpawnRunnerWave();
            lastWaveType = WaveType.RunnerWave;
            consecutiveToughWaves++;
        }
        else if (roll < lootWaveChance && lastWaveType != WaveType.Loot)
        {
            SpawnLootWave();
            lastWaveType = WaveType.Loot;
            consecutiveToughWaves = 0; // Reset the counter
        }
        else
        {
            SpawnGauntletWave();
            lastWaveType = WaveType.Gauntlet;
            consecutiveToughWaves++;
        }

        nextSpawnZ += distanceBetweenRows;
    }

    void SpawnGauntletWave()
    {
        int zombiesToSpawn = startZombies + (waveNumber / 4);
        zombiesToSpawn = Mathf.Min(zombiesToSpawn, maxZombies);

        // Calculate health and apply the cap.
        float zombieHealthForThisWave = baseZombieHealth + (waveNumber * healthPerWave);
        zombieHealthForThisWave = Mathf.Min(zombieHealthForThisWave, maxNormalZombieHealth);

        int gateLane;
        if (lastThreatLane != -1 && Random.value < 0.75f)
        {
            gateLane = (lastThreatLane == 0) ? 1 : 0;
        }
        else
        {
            gateLane = Random.Range(0, 2);
        }
        lastThreatLane = gateLane;

        int gateIndex = Random.Range(0, gatePrefabs.Length);
        float gateX = (gateLane - 0.5f) * laneDistance;
        Vector3 gatePos = new Vector3(gateX, 1f, nextSpawnZ + (zombiesToSpawn * zombieSpacing));
        Instantiate(gatePrefabs[gateIndex], gatePos, Quaternion.identity);

        int zombieLane = (gateLane == 0) ? 1 : 0;
        float baseZombieX = (zombieLane - 0.5f) * laneDistance;
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            float zOffset = i * zombieSpacing;
            Vector3 zombiePos = new Vector3(baseZombieX, 1f, nextSpawnZ + zOffset);
            SpawnSingleZombie(zombiePos, zombieHealthForThisWave, false, zombiePrefab);
        }
    }

    // ... (rest of the Spawner script is the same, no other changes needed in other functions)

    void SpawnRunnerWave()
    {
        int runnersToSpawn = (waveNumber > 8) ? 2 : 1;
        for (int i = 0; i < runnersToSpawn; i++)
        {
            int randomLane = Random.Range(0, 2);
            float zOffset = (i == 1) ? zombieSpacing * 1.5f : 0;
            float runnerX = (randomLane - 0.5f) * laneDistance;
            Vector3 runnerPos = new Vector3(runnerX, 1f, nextSpawnZ + zOffset);
            SpawnSingleZombie(runnerPos, 75f, false, runnerZombiePrefab);
        }
    }

    void SpawnEliteStandoffWave()
    {
        float eliteHealth = baseZombieHealth + (waveNumber * healthPerWave);
        int normalZombieCount = 3;
        float normalZombieHealth = baseZombieHealth + ((waveNumber / 2) * healthPerWave);
        normalZombieHealth = Mathf.Min(normalZombieHealth, maxNormalZombieHealth);

        int eliteLane = Random.Range(0, 2);
        lastThreatLane = eliteLane;
        int normalLane = (eliteLane == 0) ? 1 : 0;

        float eliteX = (eliteLane - 0.5f) * laneDistance;
        Vector3 elitePos = new Vector3(eliteX, 1f, nextSpawnZ + zombieSpacing);
        SpawnSingleZombie(elitePos, eliteHealth, true, zombiePrefab);

        float normalX = (normalLane - 0.5f) * laneDistance;
        for (int i = 0; i < normalZombieCount; i++)
        {
            float zOffset = i * zombieSpacing;
            Vector3 normalPos = new Vector3(normalX, 1f, nextSpawnZ + zOffset);
            SpawnSingleZombie(normalPos, normalZombieHealth, false, zombiePrefab);
        }
    }

    void SpawnLootWave()
    {
        int scrapToSpawn = 3;
        for (int i = 0; i < scrapToSpawn; i++)
        {
            int randomLane = Random.Range(0, 2);
            float scrapX = (randomLane - 0.5f) * laneDistance;
            float scrapZ = nextSpawnZ + (i * (zombieSpacing / 2));
            Vector3 scrapPos = new Vector3(scrapX, 0.5f, scrapZ);
            Instantiate(scrapPrefab, scrapPos, Quaternion.identity);
        }
    }

    void SpawnSingleZombie(Vector3 position, float health, bool makeElite, GameObject prefabToSpawn)
    {
        float xWobble = Random.Range(-0.5f, 0.5f);
        position.x += xWobble;

        GameObject zombieObj = Instantiate(prefabToSpawn, position, Quaternion.identity);

        var normalLogic = zombieObj.GetComponent<ZombieLogic>();
        if (normalLogic != null)
        {
            normalLogic.SetInitialHealth(health);
            if (makeElite)
            {
                normalLogic.MakeElite(eliteColor);
            }
            return;
        }

        var runnerLogic = zombieObj.GetComponent<RunnerZombieLogic>();
        if (runnerLogic != null)
        {
            runnerLogic.SetInitialHealth(health);
            if (makeElite)
            {
                runnerLogic.MakeElite(eliteColor);
            }
        }
    }
}
