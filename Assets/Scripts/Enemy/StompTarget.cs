using UnityEngine;

public class StompTarget : MonoBehaviour
{
    private Enemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();

        // make stomp collider slippery
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            PhysicMaterial slippery = new PhysicMaterial();
            slippery.dynamicFriction = 0f;
            slippery.staticFriction = 0f;
            slippery.bounciness = 0f;
            slippery.frictionCombine = PhysicMaterialCombine.Minimum;
            col.material = slippery;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMotor player = other.GetComponent<PlayerMotor>();
        if (player == null) return;

        if (player.GetVelocityY() < 0)
            player.currentStompTarget = enemy;              // only set target, never touch canStomp
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMotor player = other.GetComponent<PlayerMotor>();
        if (player == null) return;

        if (player.currentStompTarget == enemy)
            player.currentStompTarget = null;              // only clear target, never touch canStomp
    }
}