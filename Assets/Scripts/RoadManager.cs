using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [Header("References")]
    public GameObject roadPrefab;
    public Transform playerTransform;

    [Header("Road Settings")]
    public int numberOfTiles = 3; // How many road tiles to have active at once
    public float tileLength = 50f; // The length of a single road tile (from its scale)

    private List<GameObject> activeTiles = new List<GameObject>();
    private float spawnZ = 0f; // The Z position where the next tile will spawn

    void Start()
    {
        // Spawn the initial set of tiles when the game starts
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        // This checks if the player has moved far enough to trigger the next tile move.
        // If the player's Z position is greater than the point where the first tile ends...
        if (playerTransform.position.z > spawnZ - (numberOfTiles * tileLength))
        {
            // ...move the oldest tile to the front.
            MoveTile();
        }
    }

    // Spawns one new tile at the end of the road
    private void SpawnTile()
    {
        GameObject newTile = Instantiate(roadPrefab, Vector3.forward * spawnZ, Quaternion.identity);
        activeTiles.Add(newTile);
        spawnZ += tileLength;
    }

    // Moves the oldest tile (at the back) to the front of the line
    private void MoveTile()
    {
        GameObject oldestTile = activeTiles[0]; // Get the first tile in the list
        activeTiles.RemoveAt(0); // Remove it from the start of the list
        oldestTile.transform.position = Vector3.forward * spawnZ; // Move it to the front
        activeTiles.Add(oldestTile); // Add it to the end of the list
        spawnZ += tileLength; // Update the position for the next spawn
    }
}