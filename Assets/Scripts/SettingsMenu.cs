using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private GameObject previousScreen;
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider paintSlider;// ADD THIS

    public static float MouseSensitivity { get; private set; } = 1f;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (controlsText != null)
            controlsText.text = GetControlsText();

        if (paintSlider != null)
        {
            paintSlider.minValue = 0f;
            paintSlider.maxValue = 5f;
            paintSlider.value = PlayerPrefs.GetFloat("PaintAmount", 1f);
            PaintSplatter.SplatterMultiplier = paintSlider.value;
            paintSlider.onValueChanged.AddListener(OnPaintChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f); // load saved volume
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            AudioListener.volume = volumeSlider.value;      // apply on start
        }

        if (sensitivitySlider != null)                      // ADD THIS
        {
            sensitivitySlider.minValue = 0.1f;
            sensitivitySlider.maxValue = 5f;
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1f);
            MouseSensitivity = sensitivitySlider.value;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;                       // affects all audio in scene
        PlayerPrefs.SetFloat("Volume", value);              // save setting
        PlayerPrefs.Save();
    }

    private void OnPaintChanged(float value)
    {
        PaintSplatter.SplatterMultiplier = value;
        PlayerPrefs.SetFloat("PaintAmount", value);
        PlayerPrefs.Save();
    }

    private void OnSensitivityChanged(float value)         // ADD THIS
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)  // ADD THIS: direct keyboard check
            Back();
    }

    public void Show(GameObject caller)
    {
        previousScreen = caller;
        previousScreen.SetActive(false);
        gameObject.SetActive(true);
        GameUI.Instance?.HideRPartHUD();
    }

    public void Back()
    {
        gameObject.SetActive(false);

        if (previousScreen != null)
        {
            previousScreen.SetActive(true);

            if (previousScreen.GetComponent<PauseMenu>() != null)
                GameUI.Instance?.ShowHUD();
            else
                GameUI.Instance?.HideHUD();
        }
        else
        {
            // try static instance first
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.gameObject.SetActive(true);
                return;
            }

            // fallback: find by name including inactive
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in all)
            {
                if (obj.name == "Main Menu" && obj.scene.isLoaded)
                {
                    obj.SetActive(true);
                    break;
                }
            }
        }
    }

    private string GetControlsText()
    {
        return "CONTROLS\n\n" +
                "Move                              WASD\n" +
                "Look                               Mouse\n" +
                "Jump                              Space\n" +
                "      -Stomp                      Space\n" +
        "Toggle Sprint                   Shift\n" +
                "Toggle Crouch                 C\n" +
                "Interact                            E\n" +
                "Attack                              Left Click\n" +
                "Throw Barrel                    Right Click\n" +
                "Toggle Placement Mode Q\n" +
                "      -Place                        Left Click\n" +
                "Pause                              ESC\n" +
                "      -Back                         ESC\n";
    }
}