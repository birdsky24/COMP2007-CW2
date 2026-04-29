using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator barrelAnimator;
    private InputManager inputManager;
    private PlacementMode placementMode;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        placementMode = FindObjectOfType<PlacementMode>();  // ADD THIS
    }

    void Update()
    {
        if (placementMode != null && placementMode.isActive) return;  // ADD THIS: block swing in placement mode

        if (inputManager.onFoot.Attack.triggered)
        {
            barrelAnimator.SetTrigger("attack");
        }
    }
}