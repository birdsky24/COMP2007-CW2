using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private GameObject nameEntryPanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private int maxEntries = 10;

    public static Leaderboard Instance;
    private int pendingScore;
    private string pendingTime;
    private int pendingZombies;
    private float pendingRawTime;

    [SerializeField] private bool infiniteMode = false;
    [SerializeField] private UnityEngine.UI.Button confirmButton;

    private string LeaderboardKey => infiniteMode ? "Leaderboard_Infinite" : "Leaderboard_Normal"; // ADD THIS

    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public string time;
        public int zombiesKilled;
        public float rawTime;
        public int score;
    }

    [Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }

    private LeaderboardData data = new LeaderboardData();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadLeaderboard();
        UpdateDisplay();
        if (nameEntryPanel != null)
            nameEntryPanel.SetActive(false);
        if (nameInputField != null)
        {
            nameInputField.onValueChanged.AddListener(ForceUppercase);
            nameInputField.onSubmit.AddListener(_ => ConfirmName());
        }
    }

    public string GetLeaderboardDisplay()
    {
        if (data.entries.Count == 0)
            return "LEADERBOARD\n\nNo entries yet";

        string display = "LEADERBOARD\n\n";
        for (int i = 0; i < data.entries.Count; i++)
        {
            LeaderboardEntry entry = data.entries[i];
            display += (i + 1) + ". " + entry.playerName +
                       "  " + entry.score + " pts" +
                       "  " + entry.time +
                       "  " + entry.zombiesKilled + " kills\n";
        }
        return display;
    }

    private void ForceUppercase(string value)
    {
        nameInputField.SetTextWithoutNotify(value.ToUpper());
    }

    public void ShowNameEntry(string resultText)
    {
        Debug.Log("Full text received:\n" + resultText);

        string[] lines = resultText.Split('\n');
        pendingTime = "";
        pendingZombies = 0;
        pendingRawTime = 0f;
        pendingScore = 0;

        foreach (string line in lines)
        {
            string trimmed = line.Trim();

            if (trimmed.Contains("Zombies killed:"))
            {
                string val = trimmed.Replace("Zombies killed:", "").Trim();
                int.TryParse(val, out pendingZombies);
            }
            else if (trimmed.Contains("Kills:"))
            {
                string val = trimmed.Replace("Kills:", "").Trim();
                int.TryParse(val, out pendingZombies);
            }

            if (trimmed.Contains("Score:"))
            {
                string val = trimmed.Replace("Score:", "").Trim();
                int.TryParse(val, out pendingScore);
            }

            if (trimmed.Contains("Time survived:"))
            {
                pendingTime = trimmed.Replace("Time survived:", "").Trim();
                ParseTime(pendingTime);
            }
            else if (trimmed.Contains("Time:"))
            {
                pendingTime = trimmed.Replace("Time:", "").Trim();
                ParseTime(pendingTime);
            }
        }

        // fallback to ZombieCounter score if not parsed
        if (pendingScore == 0)
        {
            ZombieCounter zombieCounter = FindObjectOfType<ZombieCounter>();
            if (zombieCounter != null)
                pendingScore = zombieCounter.Score;
        }

        Debug.Log("Final - zombies: " + pendingZombies + " time: " + pendingTime + " rawTime: " + pendingRawTime + " score: " + pendingScore);

        if (nameEntryPanel != null)
        {
            nameEntryPanel.SetActive(true);
            if (confirmButton != null)
                confirmButton.interactable = true;
            nameInputField.text = "";
            nameInputField.characterLimit = 3;
            nameInputField.ActivateInputField();
        }
    }

    private void ParseTime(string time)
    {
        string[] parts = time.Split(':');
        if (parts.Length == 2)
        {
            int minutes = 0, seconds = 0;
            int.TryParse(parts[0], out minutes);
            int.TryParse(parts[1], out seconds);
            pendingRawTime = minutes * 60f + seconds;
        }
    }

    public void ConfirmName()
    {
        string playerName = nameInputField.text.ToUpper();
        if (playerName.Length == 0)
            playerName = "???";
        while (playerName.Length < 3)
            playerName += "_";

        AddEntry(playerName, pendingTime, pendingZombies, pendingRawTime, pendingScore);

        if (nameEntryPanel != null)
            nameEntryPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.interactable = false;

        UpdateAllLeaderboardDisplays();
    }

    private void AddEntry(string playerName, string formattedTime, int zombiesKilled, float rawTime, int score)
    {
        if (zombiesKilled <= 0) return;

        LeaderboardEntry entry = new LeaderboardEntry
        {
            playerName = playerName,
            time = formattedTime,
            zombiesKilled = zombiesKilled,
            rawTime = rawTime,
            score = score
        };

        data.entries.Add(entry);
        data.entries.Sort((a, b) => b.score.CompareTo(a.score)); // CHANGE: always sort by score

        if (data.entries.Count > maxEntries)
            data.entries.RemoveRange(maxEntries, data.entries.Count - maxEntries);

        SaveLeaderboard();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (leaderboardText == null) return;

        if (data.entries.Count == 0)
        {
            leaderboardText.text = "LEADERBOARD\n\nNo entries yet";
            return;
        }

        string display = "LEADERBOARD\n\n";
        for (int i = 0; i < data.entries.Count; i++)
        {
            LeaderboardEntry entry = data.entries[i];
            display += (i + 1) + ". " + entry.playerName +
                       "  " + entry.score + " pts" +
                       "  " + entry.time +
                       "  " + entry.zombiesKilled + " kills\n";
        }
        leaderboardText.text = display;
    }

    private void UpdateAllLeaderboardDisplays()
    {
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>(true);
        DeathScreen deathScreen = FindObjectOfType<DeathScreen>(true);

        if (pauseMenu != null)
            pauseMenu.UpdateLeaderboard(GetLeaderboardDisplay());

        if (deathScreen != null)
            deathScreen.UpdateLeaderboard(GetLeaderboardDisplay());
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LeaderboardKey, json);            // USE key
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LeaderboardKey))
        {
            string json = PlayerPrefs.GetString(LeaderboardKey);
            data = JsonUtility.FromJson<LeaderboardData>(json);
            data.entries.RemoveAll(e => e.zombiesKilled <= 0);
            data.entries.Sort((a, b) => b.score.CompareTo(a.score)); // CHANGE: always sort by score
        }
    }

    public void ClearLeaderboard()
    {
        data.entries.Clear();
        PlayerPrefs.DeleteKey(LeaderboardKey);                  // USE key
        UpdateDisplay();
    }
}