using UnityEngine;

public class BarrelHitbox : MonoBehaviour
{
    private Collider hitboxCollider;
    private int damage = 34;
    private int durability = 5;
    private bool isBroken = false;
    private BarrelCounter barrelCounter;
    private PlacementMode placementMode;

    [SerializeField] private Animator barrelAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip wallHitSound;
    [SerializeField] private AudioClip barrelHitSound;
    [SerializeField] private AudioClip barrelBreakSound;
    [SerializeField] private GameObject heldBarrel;

    public bool IsSwinging { get; private set; } = false;

    void Start()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false;
        hitboxCollider.isTrigger = true;
        placementMode = FindObjectOfType<PlacementMode>();
        GetBarrelCounter();
    }

    void Update()
    {

    }

    public void EnableHitbox()
    {
        if (isBroken) return;
        if (placementMode != null && placementMode.isActive) return; // don't enable in placement mode
        IsSwinging = true;
        hitboxCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        IsSwinging = false;
        hitboxCollider.enabled = false;
    }

    public void HideBarrel()
    {
        isBroken = true;
        DisableHitbox();
        if (heldBarrel != null)
            heldBarrel.SetActive(false);
    }

    public void ShowBarrel()
    {
        if (heldBarrel != null)
            heldBarrel.SetActive(true);
    }

    public void ResetDurability()                               // ADD THIS: called when barrel placed
    {
        durability = 5;
        isBroken = false;
        BarrelCounter counter = GetBarrelCounter();
        if (counter != null)
            counter.UpdateDurabilityDisplay(durability);
    }

    private BarrelCounter GetBarrelCounter()
    {
        if (barrelCounter == null)
            barrelCounter = FindObjectOfType<BarrelCounter>();
        return barrelCounter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (isBroken) return;
        if (placementMode != null && placementMode.isActive) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (enemy.currHealth <= 0) return;
            enemy.TakeDamage(damage);
            PlaySound(enemyHitSound);
            durability--;

            BarrelCounter counter = GetBarrelCounter();
            if (counter != null)
                counter.UpdateDurabilityDisplay(durability);   // update durability display

            if (durability <= 0)
                BreakBarrel();

            return;
        }

        if (other.CompareTag("Barrel"))
        {
            PlaySound(barrelHitSound);
            StopAnimation();
            return;
        }

        PlaySound(wallHitSound);
        StopAnimation();
    }

    private void BreakBarrel()
    {
        isBroken = true;
        DisableHitbox();
        PlaySound(barrelBreakSound);

        BarrelCounter counter = GetBarrelCounter();

        if (counter != null && counter.Count > 0)
        {
            counter.Decrement();
            durability = 5;
            isBroken = false;
            heldBarrel.SetActive(true);
            counter.UpdateDurabilityDisplay(durability);       // reset durability display
        }
        else
        {
            HideBarrel();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip);
    }

    private void StopAnimation()
    {
        DisableHitbox();
        barrelAnimator.SetTrigger("stopSwing");
    }
}