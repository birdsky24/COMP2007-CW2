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

    public NavMeshAgent Agent { get => agent;  }
    public GameObject Player { get => player; }
    public Animator Animator { get => animator; }

    public Path path;
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;
    public float attackDistance = 2f;

    //just for debugging purposes
    [SerializeField]
    private string currentState;
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateMachine.Initialise();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }
    public bool CanSeePlayer()
    {
        if(player != null)
        {
            //is the player close enough to be seen?
            if(Vector3.Distance(transform.position,player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitinfo = new RaycastHit();
                    if(Physics.Raycast(ray,out hitinfo, sightDistance))
                    {
                        if(hitinfo.transform.gameObject == player)
                        {
                            return true;
                        }
                    }
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                }
            }
        }
        return false;
    }
}
