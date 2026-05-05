using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    private float lockTimer = 0f;
    private float lockDuration = 0.1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ProcessLook(Vector2 input)
    {
        if (lockTimer < lockDuration)
        {
            lockTimer += Time.deltaTime;
            return;
        }

        // pick sensitivity based on active device
        float sensitivity;
        if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().magnitude > 0.01f)
            sensitivity = SettingsMenu.ControllerSensitivity * 30f;
        else
            sensitivity = SettingsMenu.MouseSensitivity * 30f;

        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * sensitivity);
    }
}