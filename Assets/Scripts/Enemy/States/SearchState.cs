using UnityEngine;

public class SearchState : BaseState
{
    private float searchTimer;
    private float moveTimer;

    public override void Enter()
    {
        enemy.Agent.speed = 4f;
        enemy.Agent.SetDestination(enemy.LastKnowPos);
        enemy.fieldOfView = 135f;
        enemy.Animator.SetBool("walk", true);
        enemy.Animator.SetBool("idle2", false);
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
            stateMachine.ChangeState(new AttackState());

        if (enemy.Agent.remainingDistance < enemy.Agent.stoppingDistance)
        {
            // standing still searching — play idle2
            enemy.Animator.SetBool("walk", false);
            enemy.Animator.SetBool("idle2", true);

            searchTimer += Time.deltaTime;
            moveTimer += Time.deltaTime;

            if (moveTimer > Random.Range(3, 5))
            {
                // moving to new search position — play walk
                enemy.Animator.SetBool("idle2", false);
                enemy.Animator.SetBool("walk", true);
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 10));
                moveTimer = 0;
            }

            if (searchTimer > 10)
                stateMachine.ChangeState(new PatrolState());
        }
        else
        {
            // moving — play walk
            enemy.Animator.SetBool("idle2", false);
            enemy.Animator.SetBool("walk", true);
        }
    }

    public override void Exit()
    {
        enemy.fieldOfView = 85f;
        enemy.Animator.SetBool("walk", false);
        enemy.Animator.SetBool("idle2", false);
    }
}