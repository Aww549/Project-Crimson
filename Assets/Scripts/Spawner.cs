using UnityEngine;
using System.Collections.Generic;

public enum WaveType { Gauntlet, Loot }

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public GameObject zombiePrefab;
    public GameObject[] gatePrefabs;
    public GameObject scrapPrefab;
    public Transform playerTransform;

    [Header("Spawner Settings")]
    public float spawnDistance = 70f;
    public float distanceBetweenRows = 15f;
    public float laneDistance = 6f;
    public float zombieSpacing = 5f; // Increased spacing for new formation
    public float subLaneWidth = 1.5f; // Distance from lane center to column

    [Header("Wave Type Probability")]
    [Range(0, 1)]
    public float lootWaveChance = 0.25f;

    [Header("Difficulty Scaling")]
    private int waveNumber = 0;
    public int startZombies = 1;
    public int maxZombies = 6; // Increased max to allow for more pairs
    public float baseZombieHealth = 50f;
    public float healthPerWave = 2f;

    [Header("Elite Zombie Settings")]
    public int minWaveForElites = 10;
    public float eliteHealthThreshold = 100f;
    public Color eliteColor = Color.magenta;

    private float nextSpawnZ;
    private WaveType lastWaveType;
    private int lastGateLane = -1;

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

        float chance = Random.value;
        if (lastWaveType == WaveType.Loot && chance < 0.9f)
        {
            SpawnGauntletWave();
            lastWaveType = WaveType.Gauntlet;
        }
        else if (chance < lootWaveChance)
        {
            SpawnLootWave();
            lastWaveType = WaveType.Loot;
        }
        else
        {
            SpawnGauntletWave();
            lastWaveType = WaveType.Gauntlet;
        }

        nextSpawnZ += distanceBetweenRows;
    }

    void SpawnGauntletWave()
    {
        int zombiesToSpawn = startZombies + (waveNumber / 3);
        zombiesToSpawn = Mathf.Min(zombiesToSpawn, maxZombies);
        float zombieHealthForThisWave = baseZombieHealth + (waveNumber * healthPerWave);

        int eliteCount = 0;
        if (waveNumber >= minWaveForElites)
        {
            eliteCount = 1 + ((waveNumber - minWaveForElites) / 5);
            eliteCount = Mathf.Min(eliteCount, zombiesToSpawn / 2); // Cap elites to half the zombies
        }

        int gateLane;
        if (lastGateLane != -1 && Random.value < 0.75f)
        {
            gateLane = (lastGateLane == 0) ? 1 : 0;
        }
        else
        {
            gateLane = Random.Range(0, 2);
        }
        lastGateLane = gateLane;

        int gateIndex = Random.Range(0, gatePrefabs.Length);
        float gateX = (gateLane - 0.5f) * laneDistance;
        Vector3 gatePos = new Vector3(gateX, 1f, nextSpawnZ + (zombiesToSpawn * zombieSpacing));
        Instantiate(gatePrefabs[gateIndex], gatePos, Quaternion.identity);

        // --- NEW TWO-COLUMN SPAWNING LOGIC ---
        int zombieLane = (gateLane == 0) ? 1 : 0;
        float laneCenterX = (zombieLane - 0.5f) * laneDistance;
        int zombiesSpawned = 0;
        int elitesSpawned = 0;

        for (int i = 0; zombiesSpawned < zombiesToSpawn; i++)
        {
            float zOffset = i * zombieSpacing;
            bool isEliteRow = elitesSpawned < eliteCount;

            // Spawn Left Zombie
            Vector3 leftPos = new Vector3(laneCenterX - subLaneWidth, 1f, nextSpawnZ + zOffset);
            SpawnSingleZombie(leftPos, zombieHealthForThisWave, isEliteRow && Random.value < 0.5f);
            zombiesSpawned++;
            if (isEliteRow && Random.value < 0.5f) elitesSpawned++;


            // Spawn Right Zombie (if there's one left to spawn)
            if (zombiesSpawned < zombiesToSpawn)
            {
                Vector3 rightPos = new Vector3(laneCenterX + subLaneWidth, 1f, nextSpawnZ + zOffset);
                SpawnSingleZombie(rightPos, zombieHealthForThisWave, isEliteRow && elitesSpawned < eliteCount);
                zombiesSpawned++;
                if (isEliteRow) elitesSpawned++;
            }
        }
    }

    void SpawnSingleZombie(Vector3 position, float health, bool makeElite)
    {
        GameObject zombieObj = Instantiate(zombiePrefab, position, Quaternion.identity);
        ZombieLogic zombieLogic = zombieObj.GetComponent<ZombieLogic>();
        zombieLogic.SetInitialHealth(health);
        if (makeElite)
        {
            zombieLogic.MakeElite(eliteColor);
        }
    }

    void SpawnLootWave()
    {
        int scrapToSpawn = 3;
        for (int i = 0; i < scrapToSpawn; i++)
        {
            int randomLane = Random.Range(0, 2);
            float scrapX = (randomLane - 0.5f) * laneDistance;
            float scrapZ = nextSpawnZ + (i * 2.0f);
            Vector3 scrapPos = new Vector3(scrapX, 0.5f, scrapZ);
            Instantiate(scrapPrefab, scrapPos, Quaternion.identity);
        }
    }
}
