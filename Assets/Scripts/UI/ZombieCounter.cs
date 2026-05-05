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
    [SerializeField] private float multiKillWindow = 0.1f;
    [SerializeField] private float killStreakWindow = 10f;
    [SerializeField] private float difficultyInterval = 30f;
    [SerializeField] private TextMeshProUGUI multiplierText;

    private int score = 0;
    private int zombieCount;
    private int totalZombiesKilled = 0;
    private float killStreakMultiplier = 1f;
    private float lastKillTime = -999f;

    private int currentScorePerKill;
    private int currentScorePerThrowKill;

    private List<float> recentKillTimes = new List<float>();

    private WinScreen winScreen;
    private GameTimer gameTimer;

    public enum AttackType
    {
        Regular,
        Throw,
        Stomp
    }

    public int Score => score;
    public int TotalKilled => totalZombiesKilled;
    public int ZombiesRemaining => zombieCount;

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

            currentScorePerKill = Mathf.Max(1, currentScorePerKill - 1);
            currentScorePerThrowKill = Mathf.Max(1, currentScorePerThrowKill - 2);

            Debug.Log("Score per kill adjusted: " + currentScorePerKill + " throw: " + currentScorePerThrowKill);
        }
    }

    public void AddScore(AttackType attackType, bool isBigZombie = false)
    {
        float now = Time.time;

        // streak logic (soft scaling)
        if (now - lastKillTime <= killStreakWindow)
            killStreakMultiplier += 0.25f * Mathf.Clamp01(5f / killStreakMultiplier);
        else
            killStreakMultiplier = 1f;

        lastKillTime = now;

        // multi-kill tracking
        recentKillTimes.Add(now);
        recentKillTimes.RemoveAll(t => now - t > multiKillWindow);
        int multiKillCount = recentKillTimes.Count;

        // base score (restores difficulty scaling)
        int baseScore = attackType switch
        {
            AttackType.Throw => currentScorePerThrowKill,
            AttackType.Stomp => 200,
            _ => currentScorePerKill
        };

        if (isBigZombie) baseScore *= 3;

        int earnedScore = Mathf.RoundToInt(baseScore * multiKillCount * killStreakMultiplier);
        score += earnedScore;

        bool isThrow = attackType == AttackType.Throw;
        bool isStomp = attackType == AttackType.Stomp;

        ShowKillFeedback(multiKillCount, isThrow, isBigZombie, earnedScore, isStomp);

        UpdateMultiplierDisplay();
        UpdateDisplay();
    }

    public void AddPickupScore(int amount)
    {
        score += amount;
        UpdateDisplay();
    }

    private void ShowKillFeedback(int multiKillCount, bool isThrow, bool isBig, int earned, bool isStomp = false)
    {
        if (ScorePopup.Instance == null) return;

        string message = "";
        Color color = new Color(0x67 / 255f, 0x67 / 255f, 0x67 / 255f);

        // kill type
        if (isStomp)
        {
            message = "STOMP! ";
            color = new Color(1f, 0.5f, 0f);
        }
        else if (isThrow)
        {
            message = "THROW KILL! ";
            color = new Color(0f, 0.8f, 1f);
        }

        if (isBig)
        {
            message += "BIG ZOMBIE! ";
            color = new Color(1f, 0f, 0.5f);
        }

        // multi kill
        if (multiKillCount >= 5)
        {
            message += "PENTA KILL x" + multiKillCount + "!\n";
            color = Color.red;
        }
        else if (multiKillCount == 4)
        {
            message += "QUAD KILL!\n";
            color = Color.red;
        }
        else if (multiKillCount == 3)
        {
            message += "TRIPLE KILL!\n";
            color = new Color(1f, 0.3f, 0f);
        }
        else if (multiKillCount == 2)
        {
            message += "DOUBLE KILL!\n";
            color = Color.yellow;
        }

        // streak
        if (killStreakMultiplier >= 1.25f)
        {
            int streakLevel = Mathf.FloorToInt((killStreakMultiplier - 1f) / 0.25f) + 1;

            message += streakLevel + "x STREAK " + "\n";

            float t = Mathf.Clamp01((streakLevel - 2) / 10f);
            Color streakColor = Color.Lerp(Color.yellow, Color.red, t);

            color = Color.Lerp(color, streakColor, 0.7f);

            if (streakLevel > 10)
            {
                float pulse = Mathf.PingPong(Time.time * 3f, 1f);
                Color pulseColor = Color.Lerp(Color.red, new Color(1f, 0f, 1f), pulse);
                color = Color.Lerp(color, pulseColor, 0.7f);
            }
        }

        message += "+" + earned + " pts";

        ScorePopup.Instance.ShowPopup(message, color);
    }

    public void OnZombieDied()
    {
        zombieCount = Mathf.Max(0, zombieCount - 1);
        totalZombiesKilled++;
        UpdateDisplay();

        if (!infiniteMode && zombieCount <= 0)
        {
            if (gameTimer != null)
            {
                // higher time bonus — reward finishing quickly
                float timeBonus = Mathf.Max(0, 600f - gameTimer.ElapsedTime) * 10f; // ADD THIS: 10x time value
                score += Mathf.RoundToInt(timeBonus);
            }
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

        float t = Mathf.Clamp01((killStreakMultiplier - 1f) / 3f);
        multiplierText.color = Color.Lerp(new Color(0x67 / 255f, 0x67 / 255f, 0x67 / 255f), Color.red, t);
    }

    void Update()
    {
        if (Time.time - lastKillTime > killStreakWindow && killStreakMultiplier > 1f)
        {
            killStreakMultiplier = 1f;
            recentKillTimes.Clear();
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