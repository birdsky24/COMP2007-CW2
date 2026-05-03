using UnityEngine;
using System.Collections;

public class Barrel : Interactable
{
    private BarrelCounter counter;
    private HealthBarScript healthBar;
    private ZombieCounter zombieCounter;                    // ADD THIS
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;

    void Start()
    {
        counter = FindObjectOfType<BarrelCounter>();
        healthBar = FindObjectOfType<HealthBarScript>();
        zombieCounter = FindObjectOfType<ZombieCounter>();  // ADD THIS
    }

    protected override void Interact()
    {
        if (counter != null) counter.Increment();
        if (healthBar != null) healthBar.AddHealth();
        if (zombieCounter != null) zombieCounter.AddPickupScore(50); // ADD THIS

        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);
        StartCoroutine(DestroyAfterSound());
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(pickupSound != null ? pickupSound.length : 0f);
        Destroy(gameObject);
    }
}