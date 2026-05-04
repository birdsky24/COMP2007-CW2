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
    private PaintSplatter paintSplatter;
    private PlayerEffects playerEffects;
    private Transform playerTransform;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    // start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
        deathScreen = FindObjectOfType<DeathScreen>(true);
        playerEffects = FindObjectOfType<PlayerEffects>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;                     // ADD THIS
        paintSplatter = player.GetComponent<PaintSplatter>();
        if (paintSplatter == null)
            paintSplatter = player.GetComponentInChildren<PaintSplatter>();
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
            paintSplatter?.StopBleeding();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);  // slight pitch variation
        audioSource.PlayOneShot(clip, 2f);
    }

    public void TakeDamage(int damageAmount = 20)
    {
        currHealth -= damageAmount;
        PlaySound(hitSound);
        if (playerEffects != null)
            playerEffects.OnDamage();
        if (paintSplatter != null)
            paintSplatter.SplatterOnHit(playerTransform.position); // USE playerTransform

        if (currHealth <= 35 && currHealth > 0)             // ADD THIS
            paintSplatter.StartBleeding();
        else
            paintSplatter.StopBleeding();
    }

    // Heal 30 health
    public void AddHealth()
    {
        currHealth += 30;
        if (currHealth >= 35)
            paintSplatter.StopBleeding();
    }
}