using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private GameObject previousScreen;
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private Slider volumeSlider;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (controlsText != null)
            controlsText.text = GetControlsText();

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f); // load saved volume
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            AudioListener.volume = volumeSlider.value;      // apply on start
        }
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;                       // affects all audio in scene
        PlayerPrefs.SetFloat("Volume", value);              // save setting
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
            previousScreen.SetActive(true);
        GameUI.Instance?.ShowRPartHUD();
    }

    private string GetControlsText()
    {
        return "CONTROLS\n\n" +
                "Move                              WASD\n" +
                "Look                               Mouse\n" +
                "Jump                              Space\n" +
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