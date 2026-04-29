using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // need this for slider
using TMPro; // need this for text mesh pro

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarValueText; // The text that says 100/100

    public int maxHealth; // maximum health
    public int currHealth; // current health

    private DeathScreen deathScreen;
    private bool isDead = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    // start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth; // set the current health to max health
        deathScreen = FindObjectOfType<DeathScreen>(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp health between 0 and 100
        currHealth = Mathf.Clamp(currHealth, 0, maxHealth);

        // set the health bar text
        healthBarValueText.text = currHealth.ToString() + "/" + maxHealth.ToString();

        // set the slider values
        healthBarSlider.value = currHealth;
        healthBarSlider.maxValue = maxHealth;

        if (currHealth <= 0 && !isDead)
        {
            isDead = true;
            deathScreen.Show();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);  // slight pitch variation
        audioSource.PlayOneShot(clip);
    }

    // Take 20 damage
    public void TakeDamage()
    {
        currHealth -= 20;
        PlaySound(hitSound);
    }

    // Take 40 damage
    public void TakeDamage40()
    {
        currHealth -= 40;
        PlaySound(hitSound);
    }

    // Take 60 damage
    public void TakeDamage60()
    {
        currHealth -= 60;
        PlaySound(hitSound);
    }

    // Heal 30 health
    public void AddHealth()
    {
        currHealth += 30;
    }
}