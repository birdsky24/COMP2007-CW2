using System.Collections.Generic;
using UnityEngine;

public class PaintSplatter : MonoBehaviour
{
    [SerializeField] private GameObject[] paintDecalPrefabs;    // ADD THIS: array of colours
    [SerializeField] private int hitSplatterCount = 4;          // fewer on hit
    [SerializeField] private int deathSplatterCount = 12;       // more on death
    [SerializeField] private float hitSplatterRadius = 0.5f;    // smaller on hit
    [SerializeField] private float deathSplatterRadius = 1f;    // larger on death
    [SerializeField] private LayerMask surfaceMask;
    [SerializeField] private int maxDecals = 500;
    private Transform paintParent;

    private static Queue<GameObject> allDecals = new Queue<GameObject>();

    void Start()
    {
        GameObject paintObject = GameObject.Find("Paint");
        if (paintObject != null)
            paintParent = paintObject.transform;
        else
        {
            // create it if it doesn't exist
            paintParent = new GameObject("Paint").transform;
        }
    }

    public void SplatterOnHit(Vector3 position)
    {
        Splatter(position, hitSplatterCount, hitSplatterRadius);
    }

    public void SplatterOnDeath(Vector3 position)
    {
        Splatter(position, deathSplatterCount, deathSplatterRadius);
    }

    private void Splatter(Vector3 position, int count, float radius)
    {
        if (paintDecalPrefabs.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 rayOrigin = new Vector3(
                position.x + randomCircle.x,
                position.y + 1f,
                position.z + randomCircle.y);

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5f, surfaceMask))
                SpawnDecal(hit.point, hit.normal);

            Vector3 outwardDirection = new Vector3(randomCircle.x, 0, randomCircle.y).normalized;
            Ray wallRay = new Ray(position + Vector3.up * 0.5f, outwardDirection);
            RaycastHit wallHit;

            if (Physics.Raycast(wallRay, out wallHit, radius, surfaceMask))
                SpawnDecal(wallHit.point, wallHit.normal);
        }
    }

    private void SpawnDecal(Vector3 position, Vector3 normal)
    {
        while (allDecals.Count >= maxDecals)
        {
            GameObject oldest = allDecals.Dequeue();
            if (oldest != null)
                Destroy(oldest);
        }

        GameObject randomPrefab = paintDecalPrefabs[Random.Range(0, paintDecalPrefabs.Length)];
        Quaternion rotation = Quaternion.LookRotation(-normal);
        rotation *= Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        GameObject decal = Instantiate(randomPrefab, position + normal * 0.01f, rotation);
        decal.transform.localScale = Vector3.one * Random.Range(0.1f, 0.4f);
        decal.transform.SetParent(paintParent);                 // ADD THIS: parent to Paint object

        allDecals.Enqueue(decal);
    }
}