using UnityEngine;

public class StompTarget : MonoBehaviour
{
    [SerializeField] private float playerBounceForce = 8f;
    private Enemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        enemy.TakeDamage(20, false);

        // launch player in their facing direction
        PlayerMotor playerMotor = other.GetComponent<PlayerMotor>();
        if (playerMotor != null)
            playerMotor.LaunchPlayer(playerBounceForce);

        // paint splatter
        PaintSplatter paintSplatter = enemy.GetComponent<PaintSplatter>();
        if (paintSplatter != null)
        {
            paintSplatter.SplatterOnHit(enemy.transform.position);
            if (enemy.currHealth <= 35 && enemy.currHealth > 0)
                paintSplatter.StartBleeding();
        }
    }
}