using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum WaveType { Gauntlet, Loot, EliteStandoff, RunnerWave }

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public GameObject zombiePrefab;
    public GameObject runnerZombiePrefab;
    public GameObject[] gatePrefabs;
    public GameObject scrapPrefab;
    public Transform playerTransform;
    public GameObject helplessSurvivorPrefab;

    [Header("Survivor Archetypes")]
    [Tooltip("Drag all COMMON SurvivorArchetype assets here.")]
    public List<SurvivorArchetype> commonArchetypes;
    [Tooltip("Drag all UNCOMMON SurvivorArchetype assets here.")]
    public List<SurvivorArchetype> uncommonArchetypes;
    [Tooltip("Drag all LEGENDARY SurvivorArchetype assets here.")]
    public List<SurvivorArchetype> legendaryArchetypes;

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
    [Range(0, 1)]
    public float baseSurvivorSpawnChance = 0.1f; // The base chance before the pity timer.

    [Header("Difficulty Scaling")]
    private int waveNumber = 0;
    public int startZombies = 1;
    public int maxZombies = 5;
    public float baseZombieHealth = 60f;
    public float healthPerWave = 1.5f;
    public float maxNormalZombieHealth = 200f;

    [Header("Elite Zombie Settings")]
    public int minWaveForElites = 10;
    public Color eliteColor = Color.magenta;

    private int consecutiveToughWaves = 0;
    private float nextSpawnZ;
    private WaveType lastWaveType;
    private int lastThreatLane = -1;

    // DEFINITIVE FIX: This flag tracks if we've spawned a survivor in this run,
    // preventing the race condition.
    private bool survivorHasBeenSpawnedThisRun = false;

    void Start()
    {
        nextSpawnZ = transform.position.z;
        lastWaveType = WaveType.Gauntlet;
        survivorHasBeenSpawnedThisRun = false; // Reset the flag at the start of each run.
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

        if (consecutiveToughWaves >= 3)
        {
            SpawnLootWave();
            lastWaveType = WaveType.Loot;
            consecutiveToughWaves = 0;
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
            consecutiveToughWaves = 0;
        }
        else
        {
            SpawnGauntletWave();
            lastWaveType = WaveType.Gauntlet;
            consecutiveToughWaves++;
        }

        nextSpawnZ += distanceBetweenRows;
    }

    void SpawnLootWave()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager instance not found in SpawnLootWave. Spawning only scrap.");
            for (int i = 0; i < 3; i++)
            {
                int randomLane = Random.Range(0, 2);
                float xPos = (randomLane - 0.5f) * laneDistance;
                float zPos = nextSpawnZ + (i * (zombieSpacing / 2));
                Vector3 spawnPos = new Vector3(xPos, 0.5f, zPos);
                Instantiate(scrapPrefab, spawnPos, Quaternion.identity);
            }
            return;
        }

        int scrapToSpawn = 3;

        // DEFINITIVE FIX: We now check our internal flag instead of the GameDataManager.
        // This prevents the race condition where multiple survivors are spawned before the first is picked up.
        bool canSpawnSurvivor = !survivorHasBeenSpawnedThisRun;

        bool survivorWillSpawn = false;
        int survivorSpawnIndex = -1;
        float currentSurvivorChance = baseSurvivorSpawnChance + GameDataManager.Instance.gameData.survivorPityChance;

        if (canSpawnSurvivor && Random.value < currentSurvivorChance)
        {
            survivorWillSpawn = true;
            survivorSpawnIndex = Random.Range(0, scrapToSpawn);
            GameDataManager.Instance.gameData.survivorPityChance = 0f;

            // DEFINITIVE FIX: Set the flag to true the moment we decide to spawn a survivor.
            survivorHasBeenSpawnedThisRun = true;
        }

        for (int i = 0; i < scrapToSpawn; i++)
        {
            int randomLane = Random.Range(0, 2);
            float xPos = (randomLane - 0.5f) * laneDistance;
            float zPos = nextSpawnZ + (i * (zombieSpacing / 2));
            Vector3 spawnPos = new Vector3(xPos, 0.5f, zPos);

            if (survivorWillSpawn && i == survivorSpawnIndex)
            {
                SurvivorArchetype chosenArchetype = SelectRandomArchetype();
                GameObject survivorObj = Instantiate(helplessSurvivorPrefab, spawnPos, Quaternion.identity);
                var pickupScript = survivorObj.GetComponent<HelplessSurvivorPickup>();
                if (pickupScript != null)
                {
                    pickupScript.SetArchetype(chosenArchetype);
                }
            }
            else
            {
                Instantiate(scrapPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    // --- All other methods are unchanged ---
    #region
    private SurvivorArchetype SelectRandomArchetype()
    {
        float rarityRoll = Random.value;
        if (rarityRoll < 0.05f && legendaryArchetypes.Count > 0)
        {
            return legendaryArchetypes[Random.Range(0, legendaryArchetypes.Count)];
        }
        else if (rarityRoll < 0.30f && uncommonArchetypes.Count > 0)
        {
            return uncommonArchetypes[Random.Range(0, uncommonArchetypes.Count)];
        }
        else
        {
            return commonArchetypes[Random.Range(0, commonArchetypes.Count)];
        }
    }

    void SpawnGauntletWave()
    {
        int zombiesToSpawn = Mathf.Min(startZombies + (waveNumber / 4), maxZombies);
        float zombieHealthForThisWave = Mathf.Min(baseZombieHealth + (waveNumber * healthPerWave), maxNormalZombieHealth);
        int gateLane = (lastThreatLane != -1 && Random.value < 0.75f) ? (lastThreatLane == 0 ? 1 : 0) : Random.Range(0, 2);
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
        float normalZombieHealth = Mathf.Min(baseZombieHealth + ((waveNumber / 2) * healthPerWave), maxNormalZombieHealth);
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

    void SpawnSingleZombie(Vector3 position, float health, bool makeElite, GameObject prefabToSpawn)
    {
        float xWobble = Random.Range(-0.5f, 0.5f);
        position.x += xWobble;
        GameObject zombieObj = Instantiate(prefabToSpawn, position, Quaternion.identity);
        var normalLogic = zombieObj.GetComponent<ZombieLogic>();
        if (normalLogic != null)
        {
            normalLogic.SetInitialHealth(health);
            if (makeElite) { normalLogic.MakeElite(eliteColor); }
            return;
        }
        var runnerLogic = zombieObj.GetComponent<RunnerZombieLogic>();
        if (runnerLogic != null)
        {
            runnerLogic.SetInitialHealth(health);
            if (makeElite) { runnerLogic.MakeElite(eliteColor); }
        }
    }
    #endregion
}
