using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public float waitTimer;

    public override void Enter()
    {
        enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
    }

    public override void Perform()
    {
        PatrolCycle();
        if (enemy.CanSeePlayer() || enemy.isAlerted)
        {
            enemy.isAlerted = false;
            stateMachine.ChangeState(new AttackState());
        }
    }

    public override void Exit()
    {
        waitTimer = 0;
        enemy.fieldOfView = 85f;
        enemy.isAlerted = false;
        enemy.Animator.SetBool("walk", false);
        enemy.Animator.SetBool("idle2", false);
    }

    public void PatrolCycle()
    {
        if (enemy.Agent.remainingDistance < 0.2f)
        {
            // standing still — play idle
            enemy.Animator.SetBool("walk", false);
            enemy.Animator.SetBool("idle2", true);
            enemy.fieldOfView = 135f;

            waitTimer += Time.deltaTime;
            if (waitTimer > 3)
            {
                if (waypointIndex < enemy.path.waypoints.Count - 1)
                    waypointIndex++;
                else
                    waypointIndex = 0;

                enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
                waitTimer = 0;
            }
        }
        else
        {
            // moving — play walk
            enemy.Agent.speed = 4f;
            enemy.Animator.SetBool("idle2", false);
            enemy.Animator.SetBool("walk", true);
            enemy.fieldOfView = 85f;
        }
    }
}