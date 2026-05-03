using UnityEngine;
public class AttackState : BaseState
{
    private float losePlayerTimer;
    private float roarTimer = 0f;
    private float roarTimeout = 0.5f;
    private bool hasRoared = false;
    private HealthBarScript playerHealth;

    public override void Enter()
    {
        playerHealth = GameObject.FindObjectOfType<HealthBarScript>();
        enemy.fieldOfView = 180f;
        enemy.Agent.updateRotation = false;
        enemy.Agent.updateUpAxis = false;
        enemy.Agent.isStopped = true;
        hasRoared = false;
        roarTimer = 0f;

        // ADD THIS: clear patrol/search animation states before roar
        enemy.Animator.SetBool("walk", false);
        enemy.Animator.SetBool("idle2", false);
        enemy.Animator.SetBool("run", false);
        enemy.Animator.ResetTrigger("roar");
        enemy.Animator.SetTrigger("roar");
    }

    public override void Exit()
    {
        enemy.fieldOfView = 85f;
        enemy.Agent.updateRotation = true;
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
            roarTimer += Time.deltaTime;

            float distanceToPlayer = Vector3.Distance(
                enemy.transform.position, enemy.Player.transform.position);

            // REMOVE stateInfo.IsName check entirely
            if (roarTimer >= roarTimeout || distanceToPlayer < 1.5f)
            {
                hasRoared = true;
                enemy.Agent.isStopped = false;
                enemy.Animator.SetBool("run", true);
                enemy.Agent.SetDestination(enemy.Player.transform.position);
            }
            else return;
        }

        if (enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;

            // rotate on Y axis only — prevents tilting when player jumps
            Vector3 direction = (enemy.Player.transform.position - enemy.transform.position).normalized;
            direction.y = 0f;                               // lock Y axis
            if (direction != Vector3.zero)                              // ADD THIS: prevent zero vector error
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.RotateTowards(
                    enemy.transform.rotation, targetRotation, 360f * Time.deltaTime);
            }

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
                // REMOVED: enemy.transform.LookAt — was causing body tilting
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