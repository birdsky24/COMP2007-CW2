using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    private SettingsMenu settingsMenu;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        gameObject.SetActive(false);
        settingsMenu = FindObjectOfType<SettingsMenu>(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSettings()
    {
        gameObject.SetActive(false);
        settingsMenu.Show(gameObject);
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
}