using UnityEngine;
using System.Collections;

public class BarrelSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] barrelPrefabs;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float spawnRadius = 50f;      // radius around spawner center
    [SerializeField] private int maxBarrelsInScene = 10;
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private LayerMask obstacleMask;        // layers that block spawning
    private Transform pickupsParent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spawnSound;

    void Start()
    {
        GameObject pickups = GameObject.Find("Pickups");
        if (pickups != null)
            pickupsParent = pickups.transform;
        else
            pickupsParent = new GameObject("Pickups").transform;

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // count existing pickup barrels
            Barrel[] existingBarrels = FindObjectsOfType<Barrel>();
            if (existingBarrels.Length < maxBarrelsInScene)
                TrySpawnBarrel();
        }
    }

    private void TrySpawnBarrel()
    {
        // try up to 10 times to find a valid spawn point
        for (int attempt = 0; attempt < 10; attempt++)
        {
            // random point within radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 rayOrigin = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y + 50f,                // start ray high above
                transform.position.z + randomCircle.y);

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;

            // check if terrain is below
            if (Physics.Raycast(ray, out hit, 100f, terrainMask))
            {
                Vector3 spawnPoint = hit.point + Vector3.up * 0.8f;

                // check nothing is blocking the spawn point
                if (!Physics.CheckSphere(spawnPoint, 1f, obstacleMask))
                {
                    SpawnBarrel(spawnPoint);
                    return;                                 // success
                }
            }
        }
        Debug.Log("Could not find valid barrel spawn point after 10 attempts");
    }

    private void SpawnBarrel(Vector3 position)
    {
        if (barrelPrefabs.Length == 0) return;

        GameObject randomPrefab = barrelPrefabs[Random.Range(0, barrelPrefabs.Length)];
        GameObject barrel = Instantiate(randomPrefab, position, Quaternion.identity);
        if (pickupsParent != null)
            barrel.transform.SetParent(pickupsParent);

        if (spawnSound != null && audioSource != null)
        {
            audioSource.transform.position = position;          // play sound at spawn position
            audioSource.PlayOneShot(spawnSound);
        }
    }
}