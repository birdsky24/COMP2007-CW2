using UnityEngine;

public class ThrownBarrel : MonoBehaviour
{
    public int damage = 80;
    private Rigidbody rb;
    private bool hasLanded = false;
    private Transform pickupsParent;
    private int bounceCount = 0;
    private bool ignorePlayer = true;                       // ignore player until first bounce
    private GameObject player;
    private HealthBarScript playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = FindObjectOfType<HealthBarScript>();

        GameObject pickups = GameObject.Find("Pickups");
        if (pickups != null)
            pickupsParent = pickups.transform;

        AddBounceMaterial();
        AddLargerHitCollider();

        // ignore player and held barrel collisions on throw
        if (player != null)
        {
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
            Collider myCollider = GetComponent<Collider>();
            foreach (Collider col in playerColliders)
                Physics.IgnoreCollision(myCollider, col, true);
        }
    }

    void Update()
    {
        if (hasLanded) return;

        // re-enable player collision after first bounce
        if (!ignorePlayer && player != null)
        {
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
            Collider myCollider = GetComponent<Collider>();
            foreach (Collider col in playerColliders)
                Physics.IgnoreCollision(myCollider, col, false);
            ignorePlayer = true;                            // only do this once
        }
    }

    private void AddBounceMaterial()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        PhysicMaterial bounceMat = new PhysicMaterial();
        bounceMat.bounciness = 0.7f;
        bounceMat.dynamicFriction = 0f;
        bounceMat.staticFriction = 0f;
        bounceMat.bounceCombine = PhysicMaterialCombine.Maximum;
        bounceMat.frictionCombine = PhysicMaterialCombine.Minimum;
        col.material = bounceMat;
    }

    private void AddLargerHitCollider()
    {
        SphereCollider largeCol = gameObject.AddComponent<SphereCollider>();
        largeCol.radius = 1.2f;                               // larger radius for easier hitting
        largeCol.isTrigger = true;                            // trigger so it doesn't affect physics
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        // check if hit floor by tag or layer
        if (collision.gameObject.CompareTag("Floor") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            Land();
            return;
        }

        // hit player after first bounce
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth?.TakeDamage(damage);                  // FIX: pass damage amount
            bounceCount++;
            damage = Mathf.Max(0, damage - 20);
            return;
        }

        // hit enemy
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null && enemy.currHealth > 0)
        {
            enemy.TakeDamage(damage);
            bounceCount++;
            damage = Mathf.Max(0, damage - 20);
            ignorePlayer = false;
            return;
        }

        // hit wall or barrel
        bounceCount++;
        damage = Mathf.Max(0, damage - 20);
        ignorePlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasLanded) return;

        // check if hit by barrel hitbox
        BarrelHitbox hitbox = other.GetComponent<BarrelHitbox>();
        if (hitbox != null)
        {
            // redirect the barrel in the swing direction
            if (rb != null)
            {
                Camera cam = Camera.main;
                Vector3 redirectDirection = cam.transform.forward + Vector3.up * 0.3f;
                rb.velocity = redirectDirection.normalized * rb.velocity.magnitude * 1.5f; // speed boost on hit
            }
            bounceCount = 0;                                  // reset bounces so full damage again
            damage = 80;                                      // reset damage on redirect
        }
    }

    private void Land()
    {
        hasLanded = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;             // stop rolling
            rb.isKinematic = true;
        }

        if (GetComponent<Barrel>() == null)
        {
            Barrel barrel = gameObject.AddComponent<Barrel>();
            barrel.promptMessage = "Pick up Barrel";
        }

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        if (pickupsParent != null)
            transform.SetParent(pickupsParent);

        Destroy(this);
    }
}