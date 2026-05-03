using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject zombieCountUI;
    [SerializeField] private GameObject barrelCountUI;
    [SerializeField] private GameObject placementBarrelCountUI;
    [SerializeField] private GameObject durabilityUI;
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject hpBarUI;
    [SerializeField] private GameObject promptTextUI;
    [SerializeField] private GameObject cooldownUI;
    [SerializeField] private GameObject multiplierUI;
    [SerializeField] private GameObject scorePopupUI;
    [SerializeField] private GameObject stompCooldownUI;

    public static GameUI Instance;

    void Awake()
    {
        Instance = this;
    }

    private void SetActive(GameObject obj, bool active)     // ADD THIS: safe setter
    {
        if (obj != null) obj.SetActive(active);
    }

    public void HideHUD()
    {
        SetActive(zombieCountUI, false);
        SetActive(barrelCountUI, false);
        SetActive(placementBarrelCountUI, false);
        SetActive(durabilityUI, false);
        SetActive(timerUI, false);
        SetActive(crosshairUI, false);
        SetActive(hpBarUI, false);
        SetActive(promptTextUI, false);
        SetActive(cooldownUI, false);
        SetActive(scorePopupUI, false);
        SetActive(stompCooldownUI, false);
        SetActive(multiplierUI, false);
    }

    public void ShowHUD()
    {
        SetActive(zombieCountUI, true);
        SetActive(barrelCountUI, true);
        SetActive(placementBarrelCountUI, true);
        SetActive(durabilityUI, true);
        SetActive(timerUI, true);
        SetActive(crosshairUI, true);
        SetActive(hpBarUI, true);
        SetActive(promptTextUI, true);
        SetActive(cooldownUI, true);
        SetActive(scorePopupUI, true);
        SetActive(stompCooldownUI, true);
        SetActive(multiplierUI, true);
    }

    public void HidePartHUD()
    {
        SetActive(barrelCountUI, false);
        SetActive(placementBarrelCountUI, false);
        SetActive(durabilityUI, false);
        SetActive(crosshairUI, false);
        SetActive(promptTextUI, false);
    }

    public void ShowPartHUD()
    {
        SetActive(barrelCountUI, true);
        SetActive(placementBarrelCountUI, true);
        SetActive(durabilityUI, true);
        SetActive(crosshairUI, true);
        SetActive(promptTextUI, true);
    }

    public void HideRPartHUD()
    {
        SetActive(zombieCountUI, false);
        SetActive(timerUI, false);
        SetActive(hpBarUI, false);
        SetActive(cooldownUI, false);
        SetActive(scorePopupUI, false);
        SetActive(stompCooldownUI, false);
        SetActive(multiplierUI, false);
    }

    public void ShowRPartHUD()
    {
        SetActive(zombieCountUI, true);
        SetActive(timerUI, true);
        SetActive(hpBarUI, true);
        SetActive(cooldownUI, true);
        SetActive(scorePopupUI, true);
        SetActive(stompCooldownUI, true);
        SetActive(multiplierUI, true);
    }
}