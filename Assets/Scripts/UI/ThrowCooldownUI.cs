using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThrowCooldownUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI cooldownText;  // optional text showing seconds
    private BarrelThrow barrelThrow;

    void Start()
    {
        barrelThrow = FindObjectOfType<BarrelThrow>();
        fillImage.fillAmount = 1f;                          // start full
    }

    void Update()
    {
        if (barrelThrow == null) return;

        float cooldownProgress = barrelThrow.GetCooldownProgress();
        fillImage.fillAmount = cooldownProgress;
        fillImage.color = cooldownProgress >= 1f ? Color.green : Color.red;

        // optional text
        if (cooldownText != null)
        {
            if (cooldownProgress < 1f)
            {
                float remaining = barrelThrow.GetCooldownRemaining();
                cooldownText.text = remaining.ToString("F1");
            }
            else
            {
                cooldownText.text = "";
            }
        }
    }
}