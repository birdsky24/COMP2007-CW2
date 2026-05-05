using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;
    private PlacementMode placementMode;
    private PauseMenu pauseMenu;
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        placementMode = GetComponent<PlacementMode>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Sprint.performed += ctx => motor.Sprint();
        onFoot.TogglePlacement.performed += ctx => placementMode.TogglePlacementMode();
        onFoot.Menu.performed += ctx =>
        {
            DeathScreen deathScreen = FindObjectOfType<DeathScreen>(true);
            WinScreen winScreen = FindObjectOfType<WinScreen>(true);

            if (deathScreen != null && deathScreen.gameObject.activeSelf) return;
            if (winScreen != null && winScreen.gameObject.activeSelf) return;

            pauseMenu.Toggle();
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //tell the playermotorr to move using the value from out movement action.
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }
    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }
}