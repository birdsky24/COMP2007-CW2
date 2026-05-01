using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
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
