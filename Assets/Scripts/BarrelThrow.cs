using UnityEngine;

public class BarrelThrow : MonoBehaviour
{
    private InputManager inputManager;
    private PlacementMode placementMode;
    private BarrelCounter barrelCounter;
    private BarrelHitbox barrelHitbox;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip bounceSound;

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

        GameObject randomPrefab = throwBarrelPrefabs[Random.Range(0, throwBarrelPrefabs.Length)];

        // spawn further forward so it doesn't clip into nearby surfaces
        Vector3 spawnPos = cam.transform.position + cam.transform.forward * 1.5f;
        GameObject thrown = Instantiate(randomPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = thrown.GetComponent<Rigidbody>();
        if (rb == null)
            rb = thrown.AddComponent<Rigidbody>();

        // prevent barrel clipping through player collider on spawn
        Collider barrelCol = thrown.GetComponent<Collider>();
        Collider playerCol = GetComponent<Collider>();
        if (barrelCol != null && playerCol != null)
            Physics.IgnoreCollision(barrelCol, playerCol);

        ThrownBarrel thrownBarrel = thrown.AddComponent<ThrownBarrel>();
        thrownBarrel.damage = 80;
        thrownBarrel.SetBounceSound(bounceSound);

        rb.AddForce(cam.transform.forward * throwForce + Vector3.up * throwUpwardForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

        if (throwSound != null && audioSource != null)          // ADD THIS
            audioSource.PlayOneShot(throwSound);

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