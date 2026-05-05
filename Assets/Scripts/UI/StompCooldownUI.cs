using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StompCooldownUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI cooldownText;
    private PlayerMotor playerMotor;

    void Start()
    {
        playerMotor = FindObjectOfType<PlayerMotor>();
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        if (playerMotor == null) return;

        float progress = playerMotor.GetStompCooldownProgress();
        fillImage.fillAmount = progress;
        fillImage.color = progress >= 1f ? new Color(0x1F / 255f, 0x4A / 255f, 0x35 / 255f) : new Color(0x68 / 255f, 0x1F / 255f, 0x0A / 255f);

        if (cooldownText != null)
        {
            float remaining = playerMotor.GetStompCooldownRemaining();
            cooldownText.text = remaining > 0f ? remaining.ToString("F1") : "";
        }
    }
}