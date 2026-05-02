using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ZombieCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private bool infiniteMode = false;
    [SerializeField] private int baseScorePerKill = 100;
    [SerializeField] private int baseScorePerThrowKill = 150;
    [SerializeField] private float multiKillWindow = 0.1f;  // seconds for multi kill
    [SerializeField] private float killStreakWindow = 5f;   // seconds to maintain streak
    [SerializeField] private float difficultyInterval = 30f;// seconds between difficulty changes
    [SerializeField] private TextMeshProUGUI multiplierText;

    private int score = 0;
    private int zombieCount;
    private int totalZombiesKilled = 0;
    private float killStreakMultiplier = 1f;
    private float lastKillTime = -999f;
    private int currentScorePerKill;
    private int currentScorePerThrowKill;

    // multi kill tracking
    private List<float> recentKillTimes = new List<float>();

    private WinScreen winScreen;
    private GameTimer gameTimer;

    public int Score { get => score; }
    public int TotalKilled { get => totalZombiesKilled; }
    public int ZombiesRemaining { get => zombieCount; }

    void Start()
    {
        zombieCount = FindObjectsOfType<Enemy>().Length;
        gameTimer = FindObjectOfType<GameTimer>();
        currentScorePerKill = baseScorePerKill;
        currentScorePerThrowKill = baseScorePerThrowKill;

        if (!infiniteMode)
            winScreen = FindObjectOfType<WinScreen>(true);

        StartCoroutine(DifficultyScoreAdjustment());
        UpdateDisplay();
    }

    private IEnumerator DifficultyScoreAdjustment()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyInterval);
            currentScorePerKill = Mathf.Max(1, currentScorePerKill - 1);       // reduce by 1 every 30s
            currentScorePerThrowKill = Mathf.Max(1, currentScorePerThrowKill - 2); // reduce by 2 every 30s (1.5 rounded)
            Debug.Log("Score per kill adjusted: " + currentScorePerKill + " throw: " + currentScorePerThrowKill);
        }
    }

    public void AddScore(bool isThrowKill = false)
    {
        float now = Time.time;

        // update kill streak multiplier
        if (now - lastKillTime <= killStreakWindow)
        {
            killStreakMultiplier += 0.25f;
        }
        else
        {
            killStreakMultiplier = 1f;                       // reset streak
        }
        lastKillTime = now;

        // track multi kills
        recentKillTimes.Add(now);
        recentKillTimes.RemoveAll(t => now - t > multiKillWindow); // keep only recent kills
        int multiKillCount = recentKillTimes.Count;

        // calculate score
        int baseScore = isThrowKill ? currentScorePerThrowKill : currentScorePerKill;
        int earnedScore = Mathf.RoundToInt(baseScore * multiKillCount * killStreakMultiplier);
        score += earnedScore;

        // show feedback
        string feedback = "";
        if (multiKillCount >= 3) feedback = "TRIPLE KILL x" + multiKillCount + "! ";
        else if (multiKillCount == 2) feedback = "DOUBLE KILL! ";
        if (killStreakMultiplier > 1f) feedback += "STREAK x" + killStreakMultiplier.ToString("F2");
        if (feedback != "") Debug.Log(feedback + " +" + earnedScore + " pts");

        UpdateMultiplierDisplay();
        UpdateDisplay();
    }

    public void OnZombieDied()
    {
        zombieCount = Mathf.Max(0, zombieCount - 1);
        totalZombiesKilled++;
        UpdateDisplay();

        if (!infiniteMode && zombieCount <= 0)
        {
            if (gameTimer != null)
                score += Mathf.RoundToInt((600f - gameTimer.ElapsedTime));
            gameTimer?.Stop();
            StartCoroutine(ShowWinScreenDelay());
        }
    }

    public void OnZombieSpawned()
    {
        zombieCount++;
        UpdateDisplay();
    }

    private void UpdateMultiplierDisplay()
    {
        if (multiplierText == null) return;
        multiplierText.text = "Multiplier: x" + killStreakMultiplier.ToString("F2");
    }

    void Update()
    {
        // reset multiplier display when streak expires
        if (Time.time - lastKillTime > killStreakWindow && killStreakMultiplier > 1f)
        {
            killStreakMultiplier = 1f;
            UpdateMultiplierDisplay();
        }
    }

    private IEnumerator ShowWinScreenDelay()
    {
        yield return new WaitForSeconds(4f);
        winScreen?.Show(totalZombiesKilled, gameTimer.GetFormattedTime());
    }

    private void UpdateDisplay()
    {
        if (infiniteMode)
            counterText.text = "Kills: " + totalZombiesKilled + "  Score: " + score;
        else
            counterText.text = "Zombies: " + zombieCount + "  Score: " + score;
    }
}