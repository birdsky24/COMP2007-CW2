using UnityEngine;

public class DeathState : BaseState
{
    private float deathTimer = 0f;
    private float destroyDelay = 3f; // how long after death animation to destroy

    public override void Enter()
    {
        if (enemy.Agent.isOnNavMesh)
            enemy.Agent.isStopped = true;
        enemy.Agent.enabled = false;
        enemy.Animator.SetTrigger("death");
    }

    public override void Perform()
    {
        deathTimer += Time.deltaTime;
        if (deathTimer >= destroyDelay)
            GameObject.Destroy(enemy.gameObject);
    }

    public override void Exit()
    {
    }
}