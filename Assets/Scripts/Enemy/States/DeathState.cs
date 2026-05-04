using UnityEngine;

public class DeathState : BaseState
{
    private float deathTimer = 0f;
    private float destroyDelay = 3f;

    public override void Enter()
    {
        if (enemy.Agent.isOnNavMesh)
            enemy.Agent.isStopped = true;
        enemy.Agent.enabled = false;
        enemy.Animator.SetTrigger("death");

        // ADD THIS: disable all colliders on death so no invisible blockers remain
        Collider[] colliders = enemy.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        ZombieCounter zombieCounter = GameObject.FindObjectOfType<ZombieCounter>();
        if (zombieCounter != null)
            zombieCounter.AddScore(enemy.lastHitType, enemy.isBigZombie);
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