using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;

    public Transform player;                             // The target of the AI

    public NavMeshAgent agent;                           // Some sort of thing to make the ai understand how to move around an area

    public LayerMask whatIsGround, whatIsPlayer;  // What will tell the AI if it is walking on a proper surface, and who the target is

    public float health = 50f; //self explanatory

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attack Variables (Whatever these are called again)
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInAttackRange, playerInSightRange, playerInLineOfSight;
    bool lineOfSight;
   

    // Start is called before the first frame update

    private void Awake()
    {
        player = GameObject.Find("PlayerCamera").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
        agent.updateRotation = true;

        //Check for Sight and Attack Range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Idle();
        if (playerInSightRange && !playerInAttackRange)  MoveInOnPlayer();
        if (playerInSightRange && playerInAttackRange)   AttackPlayer();
        //if (!playerInLineOfSight && playerInAttackRange) FlankPlayer();

    }

    void Idle()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
        //They'll always be in sight range, so no need for this
    }

    private void SearchWalkPoint()
    {
        //calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    
    void MoveInOnPlayer()
    {
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        //agent.updateRotation = false;
        transform.LookAt(player.transform);

        agent.SetDestination(transform.position);
        // Right now we follow the guide, so the enemy is stationary here
        //who wants an enemy that doesnt look at you?
        Debug.Log(agent.transform.position);
        if (!alreadyAttacked)
        {
            //Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

            ///
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        

    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //void FlankPlayer()
   // {

   // }
    /*
    int LineOfSight(bool lineOfSight)
    {
        
    }
    */
}
