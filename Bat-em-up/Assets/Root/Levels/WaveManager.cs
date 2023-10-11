using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public List<Transform> spawnPoint;
        public List<GameObject> enemyPrefab;
        public int enemyCount;
        public float timeBetweenSpawns;
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private bool isSpawningWave = false;

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            currentWaveIndex++;
        }
        else
        {
            // All waves have been completed, you can handle game over or victory logic here.
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawningWave = true;
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.spawnPoint[i], wave.enemyPrefab[i]);
            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }
        isSpawningWave = false;

        // Wait for all enemies to be defeated before starting the next wave
     /*   while (GameObject.FindObjectsOfType<Enemy>().Length > 0)
        {
            yield return null;
        }*/

        // Start the next wave
        StartNextWave();
    }

    private void SpawnEnemy(Transform spawnPoint, GameObject enemyPrefab)
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void Update()
    {
        // You can implement additional wave management logic here, such as checking for player progress.
    }
}
