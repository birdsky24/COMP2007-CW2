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
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        if (barrelThrow == null) return;

        float cooldownProgress = barrelThrow.GetCooldownProgress();
        fillImage.fillAmount = cooldownProgress;
        fillImage.color = cooldownProgress >= 1f ? new Color(0x1F / 255f, 0x4A / 255f, 0x35 / 255f) : new Color(0x68 / 255f, 0x1F / 255f, 0x0A / 255f);

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