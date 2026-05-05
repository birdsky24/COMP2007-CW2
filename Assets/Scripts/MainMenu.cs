using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    private SettingsMenu settingsMenu;
    [SerializeField] private GameObject firstButton;

    void Start()
    {
        settingsMenu = FindObjectOfType<SettingsMenu>(true);
    }

    void OnEnable()
    {
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(firstButton);
    }

    public void OpenSettings()
    {
        settingsMenu.Show(gameObject);                      // passes itself as caller
    }

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1f); // ADD THIS: load saved volume
    }
    public void PlayGame()
    {
        StartCoroutine(LoadWithDelay());
    }

    private IEnumerator LoadWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(1); // Optionally: SceneManager.GetActiveScene().buildIndex + 1. Will load the next scene.
    }

    private IEnumerator LoadWithDelay3()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(2); // Optionally: SceneManager.GetActiveScene().buildIndex + 1. Will load the next scene.
    }

    public void LoadInfinite()
    {
        StartCoroutine(LoadWithDelay3());
    }

    public void QuitGame()
    {
        Application.Quit(); // Doesn't work in the editor because it could close unity and lose unsaved work
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadWithDelay2());
    }

    private IEnumerator LoadWithDelay2()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(0); // Loads main menu scene
    }
}
