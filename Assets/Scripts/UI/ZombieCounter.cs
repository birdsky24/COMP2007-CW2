using UnityEngine;
using TMPro;
using System.Collections;

public class ZombieCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    private int zombieCount;
    private int totalZombiesKilled = 0;
    private WinScreen winScreen;
    private GameTimer gameTimer;

    public int TotalKilled { get => totalZombiesKilled; }
    public int ZombiesRemaining { get => zombieCount; }

    void Start()
    {
        zombieCount = FindObjectsOfType<Enemy>().Length;
        winScreen = FindObjectOfType<WinScreen>(true);
        gameTimer = FindObjectOfType<GameTimer>();
        UpdateDisplay();
    }

    public void OnZombieDied()
    {
        zombieCount = Mathf.Max(0, zombieCount - 1);
        totalZombiesKilled++;
        UpdateDisplay();

        if (zombieCount <= 0)
        {
            gameTimer?.Stop();
            StartCoroutine(ShowWinScreenDelay());
        }
    }

    private IEnumerator ShowWinScreenDelay()
    {
        yield return new WaitForSeconds(4f);
        winScreen?.Show(totalZombiesKilled, gameTimer.GetFormattedTime());
    }

    public void OnZombieSpawned()
    {
        zombieCount++;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        counterText.text = "Zombies: " + zombieCount;
    }
}