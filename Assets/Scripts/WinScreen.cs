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

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        zombieCounter = FindObjectOfType<ZombieCounter>();
        gameTimer = FindObjectOfType<GameTimer>();
    }

    public void Show(int zombiesKilled, string time)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        resultText.text = "SUCCESS\n" + "You killed " + zombiesKilled + "\n" + " zombies in\n" + time + "!";
        GameUI.Instance?.HideHUD();
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