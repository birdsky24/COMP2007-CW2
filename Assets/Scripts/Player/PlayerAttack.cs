using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator barrelAnimator;
    private InputManager inputManager;
    private PlacementMode placementMode;

    public static bool AttackBlocked = false; // ADD THIS

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        placementMode = FindObjectOfType<PlacementMode>();
    }

    void Update()
    {
        if (AttackBlocked) return;
        if (placementMode != null && placementMode.isActive) return;

        if (inputManager.onFoot.Attack.triggered)
            barrelAnimator.SetTrigger("attack");
    }
}