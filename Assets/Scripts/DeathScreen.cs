using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    private SettingsMenu settingsMenu;
    private GameTimer gameTimer;
    private ZombieCounter zombieCounter;

    [SerializeField] private bool infiniteMode = false;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI leaderboardText;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        gameObject.SetActive(false);
        settingsMenu = FindObjectOfType<SettingsMenu>(true);
        zombieCounter = FindObjectOfType<ZombieCounter>();
        gameTimer = FindObjectOfType<GameTimer>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameUI.Instance?.HideHUD();
        PaintSplatter.BleedingPaused = true;

        if (statsText != null)
            statsText.text = "FAILURE\n" +
                             "Zombies killed: " + zombieCounter.TotalKilled + "\n" +
                             "Zombies remaining: " + zombieCounter.ZombiesRemaining + "\n" +
                             "Time survived: " + gameTimer.GetFormattedTime() + "\n" +
                             "Score: " + zombieCounter.Score;  // ADD THIS

        if (infiniteMode)
            statsText.text = "GAME OVER\n" +
                             "Zombies killed: " + zombieCounter.TotalKilled + "\n" +
                             "Time survived: " + gameTimer.GetFormattedTime() + "\n" +
                             "Score: " + zombieCounter.Score;  // ADD THIS

        if (leaderboardText != null)
            leaderboardText.text = GetLeaderboard();

        if (infiniteMode && Leaderboard.Instance != null)
            Leaderboard.Instance.ShowNameEntry(statsText.text);
    }

    private string GetLeaderboard()
    {
        Leaderboard lb = Leaderboard.Instance ?? FindObjectOfType<Leaderboard>(true);
        return lb != null ? lb.GetLeaderboardDisplay() : "LEADERBOARD\n\nNo entries yet";
    }

    public void OpenSettings()
    {
        gameObject.SetActive(false);
        settingsMenu.Show(gameObject);
    }

    public void UpdateLeaderboard(string display)
    {
        if (leaderboardText != null)
            leaderboardText.text = display;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit(); // Doesn't work in the editor because it could close unity and lose unsaved work
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadMainMenuDelay());
    }

    private IEnumerator LoadMainMenuDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);  // use realtime since timescale may be 0
        SceneManager.LoadScene(0);
    }

    private IEnumerator LoadWithDelay3()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(2);
    }

    public void LoadInfinite()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadWithDelay3());
    }
}