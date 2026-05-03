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
        fillImage.color = progress >= 1f ? Color.green : Color.red;

        if (cooldownText != null)
        {
            float remaining = playerMotor.GetStompCooldownRemaining();
            cooldownText.text = remaining > 0f ? remaining.ToString("F1") : "";
        }
    }
}