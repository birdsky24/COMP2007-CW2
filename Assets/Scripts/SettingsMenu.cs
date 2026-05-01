using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private GameObject previousScreen;
    [SerializeField] private TextMeshProUGUI controlsText;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (controlsText != null)
            controlsText.text = GetControlsText();
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
        GameUI.Instance?.HideHUD();
    }

    public void Back()
    {
        gameObject.SetActive(false);
        if (previousScreen != null)
            previousScreen.SetActive(true);
        GameUI.Instance?.ShowHUD();
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