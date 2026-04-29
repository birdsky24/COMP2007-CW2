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

    // Take 25 damage
    public void TakeDamage()
    {
        currHealth -= 20;
    }

    // Heal 25 health
    public void AddHealth()
    {
        currHealth += 20;
    }
}