using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private SettingsMenu settingsMenu;
    private GameTimer gameTimer;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        gameObject.SetActive(false);
        settingsMenu = FindObjectOfType<SettingsMenu>(true);
        gameTimer = FindObjectOfType<GameTimer>();
    }

    public void OpenSettings()
    {
        settingsMenu.Show(gameObject);
    }

    public void Toggle()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        gameTimer?.Resume();
    }

    private void Pause()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        gameTimer?.Pause();
    }

    public void Restart()
    {
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
}