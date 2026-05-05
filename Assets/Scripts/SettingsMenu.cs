using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private GameObject previousScreen;
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI gamepadControlsText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider controllerSensitivitySlider;
    [SerializeField] private Slider paintSlider;
    [SerializeField] private GameObject settingsFirstButton;
    [SerializeField] private GameObject pauseFirstButton;
    [SerializeField] private GameObject deathFirstButton;
    [SerializeField] private GameObject winFirstButton;
    [SerializeField] private GameObject mainFirstButton;
    [SerializeField] private GameObject showObject;   // drag object to show here
    [SerializeField] private GameObject hideObject;   // drag object to hide here
    [SerializeField] private UnityEngine.UI.Toggle toggle;

    public static float MouseSensitivity { get; private set; } = 1f;
    public static float ControllerSensitivity { get; private set; } = 20f;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (toggle != null)
        {
            toggle.isOn = PlayerPrefs.GetInt("ToggleSetting", 1) == 1;
            ApplyToggle(toggle.isOn);
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        if (controlsText != null)
            controlsText.text = GetControlsText();
        if (gamepadControlsText != null)
            gamepadControlsText.text = GetGamepadControlsText();

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

        if (controllerSensitivitySlider != null)
        {
            controllerSensitivitySlider.minValue = 0.1f;
            controllerSensitivitySlider.maxValue = 30f;
            controllerSensitivitySlider.value = PlayerPrefs.GetFloat("ControllerSensitivity", 20f);
            ControllerSensitivity = controllerSensitivitySlider.value;
            controllerSensitivitySlider.onValueChanged.AddListener(OnControllerSensitivityChanged);
        }
    }

    private void OnControllerSensitivityChanged(float value)
    {
        ControllerSensitivity = value;
        PlayerPrefs.SetFloat("ControllerSensitivity", value);
        PlayerPrefs.Save();
    }

    private void OnToggleChanged(bool value)
    {
        ApplyToggle(value);
        PlayerPrefs.SetInt("ToggleSetting", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyToggle(bool value)
    {
        if (showObject != null) showObject.SetActive(value);
        if (hideObject != null) hideObject.SetActive(!value);
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
        PlayerAttack.AttackBlocked = true;
        previousScreen = caller;
        previousScreen.SetActive(false);
        gameObject.SetActive(true);
        GameUI.Instance?.HideRPartHUD();
    }

    void OnEnable()
    {
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(settingsFirstButton);
    }

    public void Back()
    {
        gameObject.SetActive(false);

        if (previousScreen != null)
        {
            previousScreen.SetActive(true);

            if (previousScreen.GetComponent<PauseMenu>() != null)
            {
                PlayerAttack.AttackBlocked = true;
                PaintSplatter.BleedingPaused = true;
                GameUI.Instance?.ShowHUD();
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(pauseFirstButton);
            }
            else if (previousScreen.GetComponent<DeathScreen>() != null)
            {
                PlayerAttack.AttackBlocked = false;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(deathFirstButton);
            }
            else if (previousScreen.GetComponent<WinScreen>() != null)
            {
                PlayerAttack.AttackBlocked = false;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(winFirstButton);
            }
            else // main menu or other
            {
                PlayerAttack.AttackBlocked = false;
                PaintSplatter.BleedingPaused = false;
                GameUI.Instance?.HideHUD();
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(mainFirstButton);
            }
        }
        else
        {
            PlayerAttack.AttackBlocked = false;
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.gameObject.SetActive(true);
                return;
            }

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

    private string GetGamepadControlsText()
    {
        return "CONTROLS\n\n" +
                "Move                              Left Stick\n" +
                "Look                               Right stick\n" +
                "Jump                              A (Button South)\n" +
                "      -Stomp                      A (Button South)\n" +
        "Toggle Sprint                   Left Stick Press\n" +
                "Toggle Crouch                 B (Button East)\n" +
                "Interact                            X (Button West)\n" +
                "Attack                              Right Trigger\n" +
                "Throw Barrel                    Left Trigger\n" +
                "Toggle Placement Mode Left Bumper\n" +
                "      -Place                        Right Trigger\n" +
                "Pause                              Start\n" +
                "      -Back                         Start\n";
    }
}