using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] zombiePrefabs;
    [SerializeField] private GameObject[] bigZombiePrefabs;
    [SerializeField] private Path[] paths;
    [SerializeField] private float initialSpawnInterval = 5f;
    [SerializeField] private float minimumSpawnInterval = 1f;
    [SerializeField] private float spawnIntervalDecrease = 0.1f;
    [SerializeField] private int initialMaxZombies = 10;
    [SerializeField] private int maxZombiesIncrease = 2;
    [SerializeField] private float difficultyIncreaseInterval = 30f;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spawnSound;

    private float currentSpawnInterval;
    private int currentMaxZombies;
    private ZombieCounter zombieCounter;
    private Transform enemiesParent;

    void Start()
    {
        zombieCounter = FindObjectOfType<ZombieCounter>();
        currentSpawnInterval = initialSpawnInterval;
        currentMaxZombies = initialMaxZombies;

        GameObject enemiesObject = GameObject.Find("Enemy&Path");
        if (enemiesObject != null)
            enemiesParent = enemiesObject.transform;
        else
            enemiesParent = new GameObject("Enemy&Path").transform;

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
                TrySpawnZombie();
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

    private void TrySpawnZombie()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 rayOrigin = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y + 50f,
                transform.position.z + randomCircle.y);

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, terrainMask))
            {
                Vector3 spawnPoint = hit.point + Vector3.up * 0.1f;

                if (!Physics.CheckSphere(spawnPoint, 1f, obstacleMask))
                {
                    SpawnZombie(spawnPoint);
                    return;
                }
            }
        }
        Debug.Log("Could not find valid zombie spawn point after 10 attempts");
    }

    private void SpawnZombie(Vector3 position)
    {
        if (zombiePrefabs.Length == 0) return;

        GameObject zombie;

        if (bigZombiePrefabs.Length > 0 && Random.value <= 0.2f)
        {
            zombie = Instantiate(
                bigZombiePrefabs[Random.Range(0, bigZombiePrefabs.Length)],
                position, Quaternion.identity);

            Enemy enemy = zombie.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.isBigZombie = true;
                enemy.maxHealth = 300;
                enemy.currHealth = 300;
            }
        }
        else
        {
            zombie = Instantiate(
                zombiePrefabs[Random.Range(0, zombiePrefabs.Length)],
                position, Quaternion.identity);
        }

        Enemy spawnedEnemy = zombie.GetComponent<Enemy>();
        if (spawnedEnemy != null)
        {
            if (paths.Length > 0)
                spawnedEnemy.path = paths[Random.Range(0, paths.Length)];

            // ADD THIS: start in attack state since player is active
            StartCoroutine(SetAttackStateNextFrame(spawnedEnemy));
        }

        if (enemiesParent != null)
            zombie.transform.SetParent(enemiesParent);

        if (spawnSound != null && audioSource != null)
        {
            audioSource.transform.position = position;
            audioSource.PlayOneShot(spawnSound);
        }

        if (zombieCounter != null)
            zombieCounter.OnZombieSpawned();
    }

    private IEnumerator SetAttackStateNextFrame(Enemy enemy)
    {
        yield return null;                                      // wait one frame for Enemy.Start to run
        if (enemy != null && enemy.currHealth > 0)
        {
            StateMachine sm = enemy.GetComponent<StateMachine>();
            if (sm != null)
                sm.ChangeState(new AttackState());
        }
    }
}