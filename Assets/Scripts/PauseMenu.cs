using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private SettingsMenu settingsMenu;
    private GameTimer gameTimer;
    [SerializeField] private TextMeshProUGUI leaderboardText;

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
        if (settingsMenu != null && settingsMenu.gameObject.activeSelf) return;

        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        PlayerAttack.AttackBlocked = false;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        gameTimer?.Resume();
        GameUI.Instance?.ShowPartHUD();
        PaintSplatter.BleedingPaused = false;
    }

    private void Pause()
    {
        PlayerAttack.AttackBlocked = true;
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        gameTimer?.Pause();
        GameUI.Instance?.HidePartHUD();
        PaintSplatter.BleedingPaused = true;

        if (leaderboardText != null)
            leaderboardText.text = GetLeaderboard();
    }

    private string GetLeaderboard()
    {
        Leaderboard lb = Leaderboard.Instance ?? FindObjectOfType<Leaderboard>(true);
        return lb != null ? lb.GetLeaderboardDisplay() : "LEADERBOARD\n\nNo entries yet";
    }

    public void UpdateLeaderboard(string display)
    {
        if (leaderboardText != null)
            leaderboardText.text = display;
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

    private IEnumerator LoadWithDelay3()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(2);
    }

    [SerializeField] private GameObject firstButton;

    void OnEnable()
    {
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(firstButton);
    }

    public void GoToNormal()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadNormalDelay());
    }

    private IEnumerator LoadNormalDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(1);
    }

    public void LoadInfinite()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadWithDelay3());
    }
}