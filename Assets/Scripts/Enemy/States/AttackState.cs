using UnityEngine;
public class AttackState : BaseState
{
    private float losePlayerTimer;
    private bool hasRoared = false;
    private HealthBarScript playerHealth;

    public override void Enter()
    {
        playerHealth = GameObject.FindObjectOfType<HealthBarScript>();
        enemy.fieldOfView = 180f;
        enemy.Animator.SetTrigger("roar");
        enemy.Agent.isStopped = true;
        hasRoared = false;
    }

    public override void Exit()
    {
        enemy.fieldOfView = 85f;
        enemy.Animator.SetBool("run", false);
        enemy.Animator.SetBool("atack1", false);
        enemy.Animator.SetBool("atack2", false);
        enemy.Animator.SetBool("atack3", false);
        enemy.Animator.SetBool("atack4", false);
        enemy.Agent.isStopped = false;
    }

    public override void Perform()
    {
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

            Vector3 direction = (enemy.Player.transform.position - enemy.transform.position).normalized;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.RotateTowards(
                enemy.transform.rotation, targetRotation, 360f * Time.deltaTime);

            float distanceToPlayer = Vector3.Distance(
                enemy.transform.position, enemy.Player.transform.position);

            if (distanceToPlayer <= enemy.attackDistance)
            {
                enemy.Agent.isStopped = true;
                enemy.Animator.SetBool("run", false);
                enemy.Animator.SetBool("atack1", true);
                enemy.Animator.SetBool("atack2", true);
                enemy.Animator.SetBool("atack3", true);
                enemy.Animator.SetBool("atack4", true);
                enemy.transform.LookAt(enemy.Player.transform);
                // damage is now handled by Animation Event
            }
            else
            {
                enemy.Agent.isStopped = false;
                enemy.Agent.speed = 14f;
                enemy.Animator.SetBool("atack1", false);
                enemy.Animator.SetBool("atack2", false);
                enemy.Animator.SetBool("atack3", false);
                enemy.Animator.SetBool("atack4", false);
                enemy.Animator.SetBool("run", true);
                enemy.Agent.SetDestination(enemy.Player.transform.position);
            }

            enemy.LastKnowPos = enemy.Player.transform.position;
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            enemy.Animator.SetBool("run", false);
            if (losePlayerTimer > 3)
                stateMachine.ChangeState(new SearchState());
        }
    }
}