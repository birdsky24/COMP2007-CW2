using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator barrelAnimator;
    private InputManager inputManager;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        if (inputManager.onFoot.Attack.triggered)
        {
            barrelAnimator.SetTrigger("attack");
        }
    }
}