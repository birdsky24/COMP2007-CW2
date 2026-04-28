using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float attackTimer;
    private bool hasRoared = false;
    private HealthBarScript playerHealth;

    public override void Enter()
    {
        playerHealth = GameObject.FindObjectOfType<HealthBarScript>();

        // roar first before chasing
        enemy.Animator.SetTrigger("roar");
        enemy.Agent.isStopped = true;       // stop moving during roar
        hasRoared = false;
    }

    public override void Exit()
    {
        enemy.Animator.SetBool("run", false);
        enemy.Animator.SetBool("atack1", false);
        enemy.Agent.isStopped = false;
    }

    public override void Perform()
    {
        // wait for roar to finish before chasing
        if (!hasRoared)
        {
            AnimatorStateInfo stateInfo = enemy.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("roar") && stateInfo.normalizedTime >= 1f)
            {
                hasRoared = true;
                enemy.Agent.isStopped = false;
                enemy.Agent.SetDestination(enemy.Player.transform.position);
            }
            else return;
        }

        if (enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;
            float distanceToPlayer = Vector3.Distance(
                enemy.transform.position, enemy.Player.transform.position);

            if (distanceToPlayer <= enemy.attackDistance)
            {
                // close enough to attack
                enemy.Agent.isStopped = true;
                enemy.Animator.SetBool("run", false);
                enemy.Animator.SetBool("atack1", true);
                enemy.Animator.SetBool("atack2", true);
                enemy.Animator.SetBool("atack3", true);
                enemy.Animator.SetBool("atack4", true);
                enemy.transform.LookAt(enemy.Player.transform);

                attackTimer += Time.deltaTime;
                if (attackTimer > 1.5f)     // hit player every 1.5 seconds
                {
                    playerHealth?.TakeDamage();
                    attackTimer = 0;
                }
            }
            else
            {
                // chase player
                enemy.Agent.isStopped = false;
                enemy.Agent.speed = 12f;
                enemy.Animator.SetBool("atack1", false);
                enemy.Animator.SetBool("atack2", false);
                enemy.Animator.SetBool("atack3", false);
                enemy.Animator.SetBool("atack4", false);
                enemy.Animator.SetBool("run", true);
                enemy.Agent.SetDestination(enemy.Player.transform.position); // UPDATE EVERY FRAME
            }
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            enemy.Animator.SetBool("run", false);
            if (losePlayerTimer > 8)
                stateMachine.ChangeState(new PatrolState());
        }
    }
}