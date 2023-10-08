using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GameObject prefabToSpawn;  // The prefab you want to spawn
    public float gameDuration = 300f; // The duration of the game in seconds (5 minutes)

    private float elapsedTime = 0f;
    private int levelNumber = 1;
    private float spawnInterval = 2f;

    void Start()
    {
        // Start spawning based on the current level number
        StartCoroutine(SpawnPrefabs());
    }

    void Update()
    {
        // Update the elapsed time
        elapsedTime += Time.deltaTime;

        // Check if the game duration has been reached
        if (elapsedTime >= gameDuration)
        {
            // Game over logic goes here
            // You can display a game over message, stop spawning, etc.
            Debug.Log("Game Over!");
            enabled = false; // Disable this script
            return;
        }
    }

    IEnumerator SpawnPrefabs()
    {
        while (true)
        {
            // Check if it's time to spawn the prefab
            yield return new WaitForSeconds(spawnInterval);
            SpawnPrefab();

            // Update the level number and adjust the spawn interval
            levelNumber++;
            spawnInterval = CalculateSpawnInterval(levelNumber);
        }
    }

    void SpawnPrefab()
    {
        // Instantiate the prefab at a random position within a certain range
        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    float CalculateSpawnInterval(int level)
    {
        // Adjust spawn interval based on the level number
        // You can define your own logic here
        return Mathf.Max(2f - level * 0.1f, 0.5f); // Example: Decrease spawn interval with each level
    }
}
