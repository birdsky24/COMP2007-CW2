using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private GameObject nameEntryPanel;     // panel with 3 letter input
    [SerializeField] private TMP_InputField nameInputField; // input field for name
    [SerializeField] private int maxEntries = 10;

    public static Leaderboard Instance;

    private string pendingTime;
    private int pendingZombies;
    private float pendingRawTime;

    [SerializeField] private UnityEngine.UI.Button confirmButton;
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public string time;
        public int zombiesKilled;
        public float rawTime;
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
            nameInputField.onValueChanged.AddListener(ForceUppercase);
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
                       "  " + entry.time +
                       "  " + entry.zombiesKilled + " kills\n";
        }
        return display;
    }

    private void ForceUppercase(string value)
    {
        nameInputField.SetTextWithoutNotify(value.ToUpper()); // ADD THIS: update without triggering listener again
    }

    public void ShowNameEntry(string resultText)
    {
        if (confirmButton != null)
            confirmButton.interactable = true;
        // parse zombies and time directly from result text
        string[] lines = resultText.Split('\n');
        pendingTime = "";
        pendingZombies = 0;
        pendingRawTime = 0f;

        foreach (string line in lines)
        {
            if (line.Contains("Zombies killed:"))
                int.TryParse(line.Replace("Zombies killed:", "").Trim(), out pendingZombies);

            if (line.Contains("Time:"))
            {
                pendingTime = line.Replace("Time:", "").Trim();
                // convert MM:SS to raw seconds for sorting
                string[] parts = pendingTime.Split(':');
                if (parts.Length == 2)
                {
                    int minutes = 0, seconds = 0;
                    int.TryParse(parts[0], out minutes);
                    int.TryParse(parts[1], out seconds);
                    pendingRawTime = minutes * 60f + seconds;
                }
            }
        }

        if (nameEntryPanel != null)
        {
            nameEntryPanel.SetActive(true);
            nameInputField.text = "";
            nameInputField.characterLimit = 3;
            nameInputField.ActivateInputField();
        }
    }

    public void ConfirmName()
    {
        string playerName = nameInputField.text.ToUpper();
        if (playerName.Length == 0)
            playerName = "???";
        while (playerName.Length < 3)
            playerName += "_";

        AddEntry(playerName, pendingTime, pendingZombies, pendingRawTime);

        if (nameEntryPanel != null)
            nameEntryPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.interactable = false;

        UpdateAllLeaderboardDisplays();
    }

    private void AddEntry(string playerName, string formattedTime, int zombiesKilled, float rawTime)
    {
        LeaderboardEntry entry = new LeaderboardEntry
        {
            playerName = playerName,
            time = formattedTime,
            zombiesKilled = zombiesKilled,
            rawTime = rawTime
        };

        data.entries.Add(entry);
        data.entries.Sort((a, b) => a.rawTime.CompareTo(b.rawTime));

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
            leaderboardText.text = "No entries yet";
            return;
        }

        string display = "LEADERBOARD\n\n";
        for (int i = 0; i < data.entries.Count; i++)
        {
            LeaderboardEntry entry = data.entries[i];
            display += (i + 1) + ". " + entry.playerName +
                       "  " + entry.time +
                       "  " + entry.zombiesKilled + " kills\n";
        }
        leaderboardText.text = display;
    }

    private void UpdateAllLeaderboardDisplays()
    {
        // update any leaderboard text in pause and death screens
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
        PlayerPrefs.SetString("Leaderboard", json);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey("Leaderboard"))
        {
            string json = PlayerPrefs.GetString("Leaderboard");
            data = JsonUtility.FromJson<LeaderboardData>(json);
        }
    }

    public void ClearLeaderboard()
    {
        data.entries.Clear();
        PlayerPrefs.DeleteKey("Leaderboard");
        UpdateDisplay();
    }
}