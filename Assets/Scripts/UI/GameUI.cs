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

    public static GameUI Instance;

    void Awake()
    {
        Instance = this;
    }

    public void HideHUD()
    {
        zombieCountUI.SetActive(false);
        barrelCountUI.SetActive(false);
        placementBarrelCountUI.SetActive(false);
        durabilityUI.SetActive(false);
        timerUI.SetActive(false);
        crosshairUI.SetActive(false);
        hpBarUI.SetActive(false);
        promptTextUI.SetActive(false);
        cooldownUI.SetActive(false);
    }

    public void ShowHUD()
    {
        zombieCountUI.SetActive(true);
        barrelCountUI.SetActive(true);
        placementBarrelCountUI.SetActive(true);
        durabilityUI.SetActive(true);
        timerUI.SetActive(true);
        crosshairUI.SetActive(true);
        hpBarUI.SetActive(true);
        promptTextUI.SetActive(true);
        cooldownUI.SetActive(true);
    }

    public void HidePartHUD()
    {
        barrelCountUI.SetActive(false);
        placementBarrelCountUI.SetActive(false);
        durabilityUI.SetActive(false);
        crosshairUI.SetActive(false);
        promptTextUI.SetActive(false);
    }

    public void ShowPartHUD()
    {
        barrelCountUI.SetActive(true);
        placementBarrelCountUI.SetActive(true);
        durabilityUI.SetActive(true);
        crosshairUI.SetActive(true);
        promptTextUI.SetActive(true);
    }
}