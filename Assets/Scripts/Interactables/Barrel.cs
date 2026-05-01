using UnityEngine;

public class Barrel : Interactable
{
    private BarrelCounter counter;
    private HealthBarScript healthBar;

    void Start()
    {
        counter = FindObjectOfType<BarrelCounter>();
        healthBar = FindObjectOfType<HealthBarScript>();
    }

    protected override void Interact()
    {
        if (counter != null)
            counter.Increment();
        if (healthBar != null)
            healthBar.AddHealth();
        Destroy(gameObject);
    }
}