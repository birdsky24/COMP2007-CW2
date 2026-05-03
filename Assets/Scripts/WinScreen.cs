using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    private GameTimer gameTimer;
    private ZombieCounter zombieCounter;
    private SettingsMenu settingsMenu;
    private Leaderboard leaderboard;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        zombieCounter = FindObjectOfType<ZombieCounter>();
        gameTimer = FindObjectOfType<GameTimer>();
        leaderboard = FindObjectOfType<Leaderboard>(true);
        settingsMenu = FindObjectOfType<SettingsMenu>(true);
    }

    public void Show(int zombiesKilled, string time)
    {
        gameObject.SetActive(true);

        if (zombieCounter == null)
            zombieCounter = FindObjectOfType<ZombieCounter>();
        if (gameTimer == null)
            gameTimer = FindObjectOfType<GameTimer>();

        GameUI.Instance?.HideHUD();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PaintSplatter.BleedingPaused = true;

        resultText.text = "VICTORY\n" +
                          "Zombies killed: " + zombiesKilled + "\n" +
                          "Time: " + time + "\n" +
                          "Score: " + (zombieCounter != null ? zombieCounter.Score.ToString() : "0");

        Leaderboard.Instance?.ShowNameEntry(resultText.text);
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

    public void OpenSettings()
    {
        gameObject.SetActive(false);
        settingsMenu.Show(gameObject);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadMainMenuDelay());
    }

    private IEnumerator LoadMainMenuDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
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