using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Minotaur : MonoBehaviour
{
    public ChaseState chaseState;

    public List<GameObject> waypointList = new List<GameObject>();
    int currentWaypointTarget;

    NavMeshAgent agent;
    Transform player;

    float cooldownBeforePatrol;
    public float breakTime;

    public Vector2 detectionBox;
    public float detectionBoxExitScaler = 1f;

    Animator animator;

    public Sprite Front, Back;
    [SerializeField] SpriteRenderer graphics;

    public bool isAttacking = false;
    public bool isCharging = false;
    [SerializeField] Vector2 chargeTarget;

    public float chargeCooldown;
    float chargeCD;

    private void Start()
    {
       
        animator = GetComponent<Animator>();

        chaseState = ChaseState.Patrol;

        waypointList.AddRange(GameObject.FindGameObjectsWithTag("Waypoint"));
        player = GameObject.Find("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        chargeCD -= Time.deltaTime;

        bool isPlayerInRange = Mathf.Abs(player.position.x - transform.position.x) < (chaseState == ChaseState.Chase ? detectionBox.x * detectionBoxExitScaler : detectionBox.x)
            && Mathf.Abs(player.position.y - transform.position.y) < (chaseState == ChaseState.Chase ? detectionBox.y * detectionBoxExitScaler : detectionBox.y);

        if (isPlayerInRange)
            chaseState = ChaseState.Chase;

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(new Vector3(player.position.x, player.position.y, 0), path);
        if (path.status == NavMeshPathStatus.PathInvalid && agent.remainingDistance > 8f)
        {
            isPlayerInRange = false;
        }

        UpdateAnimation();
        
        if(isCharging) 
        {
            agent.speed = 80;
            agent.acceleration = 80;
            agent.SetDestination(chargeTarget);
            return;
        }
        else
        {
            agent.speed = 13;
            agent.acceleration = 25;
        }

        if (isAttacking)
        {
            
            agent.SetDestination(transform.position);
            return;
        }




        if (chaseState == ChaseState.Patrol)
        {
            cooldownBeforePatrol = breakTime;

            agent.SetDestination(new Vector3(waypointList[currentWaypointTarget].transform.position.x,
            waypointList[currentWaypointTarget].transform.position.y, transform.position.z));

            if (Vector2.Distance(transform.position, waypointList[currentWaypointTarget].transform.position) < 2f)
            {
                chaseState = ChaseState.Wait;
                currentWaypointTarget = Random.Range(0, waypointList.Count - 1);
            }
        }

        if(chaseState == ChaseState.Wait)
        {
            agent.SetDestination(transform.position);
            cooldownBeforePatrol -= Time.deltaTime;
            if (cooldownBeforePatrol <= 0f)
                chaseState = ChaseState.Patrol;
        }

        if (chaseState == ChaseState.Chase)
        {
            cooldownBeforePatrol = breakTime;
            agent.SetDestination(new Vector3(player.position.x, player.position.y, transform.position.z));

            if(!isPlayerInRange)
                chaseState = ChaseState.Wait;

            if (Vector2.Distance(transform.position, player.position) < 10f)
            {
                Charge();
            }

            if (Vector2.Distance(transform.position, player.position) < 6f)
            {
                Attack();
            }

            if (Vector2.Distance(transform.position, player.position) < 4f)
                agent.SetDestination(transform.position);

        }

    }

    void Attack()
    {
        if (isAttacking) return;
        animator.SetTrigger("Attack");
    }

    void Charge()
    {
        if (isAttacking || chargeCD > 0) return;

        chargeTarget = Vector2.Lerp(transform.position, player.position, 1.9f);
        animator.SetTrigger("Charge");
        chargeCD = chargeCooldown;
    }

    void UpdateAnimation()
    {
        Vector3 dir = agent.velocity;
        if (dir.y > 0.1f)
            graphics.sprite = Back;
        else if (dir.y < -0.1f)
            graphics.sprite = Front;

        if (dir.x > 0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
        if (dir.x < -0.1f)
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, detectionBox);
    }
}
