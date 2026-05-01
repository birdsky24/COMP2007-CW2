using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    private Animator animator;
    private Vector3 lastKnowPos;

    public NavMeshAgent Agent { get => agent;  }
    public GameObject Player { get => player; }
    public Animator Animator { get => animator; }
    public Vector3 LastKnowPos { get => lastKnowPos; set => lastKnowPos = value; }

    public Path path;
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;
    public float attackDistance = 2f;
    public float hearingRange = 10f;
    public bool isAlerted = false;
    public int maxHealth = 100;
    public int currHealth;
    [SerializeField] private GameObject[] barrelDropPrefabs;
    private PaintSplatter paintSplatter;
    private Transform pickupsParent;

    private PlayerMotor playerMotor;
    private EnemySounds enemySounds;

    //just for debugging purposes
    [SerializeField]
    private string currentState;
    void Start()
    {
        paintSplatter = GetComponent<PaintSplatter>();
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemySounds = GetComponent<EnemySounds>();
        stateMachine.Initialise();
        currHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        playerMotor = player.GetComponent<PlayerMotor>();

        GameObject pickupsObject = GameObject.Find("Pickups");
        if (pickupsObject != null)
            pickupsParent = pickupsObject.transform;
        else
            pickupsParent = new GameObject("Pickups").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (currHealth <= 0) return;

        CanSeePlayer();
        CheckPlayerProximity();
        currentState = stateMachine.activeState.ToString();
    }
    public void TakeDamage(int damage)
    {
        if (currHealth <= 0) return;

        currHealth -= damage;

        if (paintSplatter != null)
            paintSplatter.SplatterOnHit(transform.position);

        if (!(stateMachine.activeState is AttackState))  // only change if not already attacking
            stateMachine.ChangeState(new AttackState());

        if (currHealth <= 0)
        {
            Die();
            return;
        } 
    }

    private void Die()
    {
        if (paintSplatter != null)
            paintSplatter.SplatterOnDeath(transform.position);
        stateMachine.ChangeState(new DeathState());
        DropBarrel();
    }

    private void DropBarrel()
    {
        if (barrelDropPrefabs.Length == 0) return;

        if (Random.value <= 0.4f)
        {
            GameObject randomBarrel = barrelDropPrefabs[Random.Range(0, barrelDropPrefabs.Length)];
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y - 0.3f,
                transform.position.z);
            GameObject pickup = Instantiate(randomBarrel, spawnPosition, Quaternion.identity);
            pickup.transform.SetParent(pickupsParent);
        }
    }

    private void CheckPlayerProximity()
    {
        if (player == null || playerMotor == null) return;
        if (currHealth <= 0) return;

        if (Vector3.Distance(transform.position, player.transform.position) < 3f
            && !playerMotor.crouching
            && !(stateMachine.activeState is AttackState))  // ADD THIS: don't trigger if already attacking
        {
            stateMachine.ChangeState(new AttackState());
        }
    }
    public void AlertFromRoar()
    {
        if (stateMachine.activeState is AttackState) return;
        isAlerted = true;
        fieldOfView = 360f;
    }
    public bool CanSeePlayer()
    {
        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitinfo;
                    if (Physics.Raycast(ray, out hitinfo, sightDistance))
                    {
                        if (hitinfo.transform.gameObject == player)
                        {
                            // player is directly visible
                            if (stateMachine.activeState is AttackState)
                                return true;

                            // in patrol or search — check if player is crouching
                            PlayerMotor playerMotor = player.GetComponent<PlayerMotor>();
                            if (playerMotor != null && playerMotor.crouching)
                                return false;   // can't see crouching player in non-attack states

                            return true;
                        }
                        else
                        {
                            // raycast hit something else (e.g. a barrel) before reaching player
                            return false;       // line of sight blocked
                        }
                    }
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                }
            }
        }
        return false;
    }
}
