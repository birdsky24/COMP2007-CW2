using UnityEngine;
using UnityEngine.UI;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private RawImage damageFlash;          // CHANGE: Image -> RawImage
    [SerializeField] private RawImage lowHealthOverlay;     // CHANGE: Image -> RawImage
    [SerializeField] private float flashDuration = 0.4f;
    [SerializeField] private float lowHealthThreshold = 30f;
    private float flashTimer = 0f;
    private bool isFlashing = false;
    private HealthBarScript healthBar;

    void Start()
    {
        healthBar = FindObjectOfType<HealthBarScript>();
        if (damageFlash != null)
            damageFlash.color = new Color(1f, 0f, 0f, 0f);
        if (lowHealthOverlay != null)
            lowHealthOverlay.color = new Color(1f, 0f, 0f, 0f);
    }

    void Update()
    {
        if (isFlashing)
        {
            flashTimer -= Time.deltaTime;
            if (damageFlash != null)
            {
                float alpha = (flashTimer / flashDuration) * 0.7f; // ADD: higher max alpha
                damageFlash.color = new Color(1f, 0f, 0f, alpha);
            }
            if (flashTimer <= 0f)
            {
                isFlashing = false;
                if (damageFlash != null)
                    damageFlash.color = new Color(1f, 0f, 0f, 0f); // ensure fully transparent
            }
        }

        if (healthBar != null && lowHealthOverlay != null)
        {
            if (healthBar.currHealth <= lowHealthThreshold)
            {
                // how close to death 0 = at threshold, 1 = at 0 health
                float healthPercent = 1f - (healthBar.currHealth / (float)lowHealthThreshold);

                // faster pulse and higher alpha the lower health gets
                float pulseSpeed = Mathf.Lerp(1.5f, 4f, healthPercent);
                float maxAlpha = Mathf.Lerp(0.2f, 0.7f, healthPercent);

                // smooth sine wave pulse
                float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

                // secondary faster flicker layered on top
                float flicker = (Mathf.Sin(Time.time * pulseSpeed * 2.7f) + 1f) / 2f;

                float alpha = Mathf.Lerp(pulse, flicker, 0.3f) * maxAlpha;
                lowHealthOverlay.color = new Color(1f, 0f, 0f, alpha);
            }
            else
            {
                // fade out smoothly when health goes back above threshold
                Color current = lowHealthOverlay.color;
                lowHealthOverlay.color = new Color(1f, 0f, 0f,
                    Mathf.Lerp(current.a, 0f, Time.deltaTime * 5f));
            }
        }
    }

    public void OnDamage()
    {
        isFlashing = true;
        flashTimer = flashDuration;
        if (damageFlash != null)
        {
            damageFlash.color = new Color(1f, 0f, 0f, 0.7f);
        }
    }
}