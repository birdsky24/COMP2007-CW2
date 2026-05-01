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
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        resultText.text = "VICTORY\n" +
                          "Zombies killed: " + zombiesKilled + "\n" +
                          "Time: " + time;

        GameUI.Instance?.HideHUD();
        Leaderboard.Instance?.ShowNameEntry(resultText.text);   // PASS resultText directly
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
}