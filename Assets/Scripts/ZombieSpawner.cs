using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] zombiePrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Path[] paths;                  // ADD THIS: drag scene paths here
    [SerializeField] private float initialSpawnInterval = 5f;
    [SerializeField] private float minimumSpawnInterval = 1f;
    [SerializeField] private float spawnIntervalDecrease = 0.1f;
    [SerializeField] private int initialMaxZombies = 10;
    [SerializeField] private int maxZombiesIncrease = 2;
    [SerializeField] private float difficultyIncreaseInterval = 30f;

    private float currentSpawnInterval;
    private int currentMaxZombies;
    private ZombieCounter zombieCounter;

    void Start()
    {
        zombieCounter = FindObjectOfType<ZombieCounter>();
        currentSpawnInterval = initialSpawnInterval;
        currentMaxZombies = initialMaxZombies;

        StartCoroutine(SpawnLoop());
        StartCoroutine(IncreaseDifficulty());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            int currentZombies = FindObjectsOfType<Enemy>().Length;
            if (currentZombies < currentMaxZombies)
                SpawnZombie();
        }
    }

    private IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);
            currentSpawnInterval = Mathf.Max(minimumSpawnInterval,
                currentSpawnInterval - spawnIntervalDecrease);
            currentMaxZombies += maxZombiesIncrease;
        }
    }

    private void SpawnZombie()
    {
        if (zombiePrefabs.Length == 0 || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject randomZombie = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];

        GameObject zombie = Instantiate(randomZombie, spawnPoint.position, spawnPoint.rotation);

        // assign a random scene path to the spawned zombie    // ADD THIS
        Enemy enemy = zombie.GetComponent<Enemy>();
        if (enemy != null && paths.Length > 0)
            enemy.path = paths[Random.Range(0, paths.Length)];

        if (zombieCounter != null)
            zombieCounter.OnZombieSpawned();
    }
}