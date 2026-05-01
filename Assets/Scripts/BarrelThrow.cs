using UnityEngine;

public class BarrelThrow : MonoBehaviour
{
    private InputManager inputManager;
    private PlacementMode placementMode;
    private BarrelCounter barrelCounter;
    private BarrelHitbox barrelHitbox;

    [SerializeField] private GameObject heldBarrel;         // drag HeldBlueBarrel here
    [SerializeField] private GameObject[] throwBarrelPrefabs; // drag your barrel prefabs here
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float throwUpwardForce = 5f;
    private float throwCooldown = 5f;
    private float lastThrowTime = -5f;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        inputManager = GetComponent<InputManager>();
        placementMode = GetComponent<PlacementMode>();
        barrelCounter = FindObjectOfType<BarrelCounter>();
        barrelHitbox = FindObjectOfType<BarrelHitbox>();
    }

    void Update()
    {
        if (placementMode != null && placementMode.isActive) return;
        if (barrelCounter == null || barrelCounter.Count <= 0) return;

        if (inputManager.onFoot.Throw.triggered)
        {
            if (Time.time >= lastThrowTime + throwCooldown)    // ADD THIS: cooldown check
            {
                Throw();
                lastThrowTime = Time.time;
            }
            else
            {
                float remaining = (lastThrowTime + throwCooldown) - Time.time;
            }
        }
    }

    private void Throw()
    {
        if (throwBarrelPrefabs.Length == 0) return;

        // pick random barrel prefab
        GameObject randomPrefab = throwBarrelPrefabs[Random.Range(0, throwBarrelPrefabs.Length)];

        // spawn at camera position
        GameObject thrown = Instantiate(randomPrefab, cam.transform.position + cam.transform.forward, Quaternion.identity);

        // add rigidbody for physics
        Rigidbody rb = thrown.GetComponent<Rigidbody>();
        if (rb == null)
            rb = thrown.AddComponent<Rigidbody>();

        // add throw component to handle damage and placement
        ThrownBarrel thrownBarrel = thrown.AddComponent<ThrownBarrel>();
        thrownBarrel.damage = 80;

        // throw in camera direction with upward arc
        rb.AddForce(cam.transform.forward * throwForce + Vector3.up * throwUpwardForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse); // random spin

        // decrement counter
        barrelCounter.Decrement();
    }

    public float GetCooldownProgress()
    {
        if (Time.time >= lastThrowTime + throwCooldown)
            return 1f;                                          // fully ready
        return (Time.time - lastThrowTime) / throwCooldown;    // 0 to 1
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, (lastThrowTime + throwCooldown) - Time.time);
    }
}