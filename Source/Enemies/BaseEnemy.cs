using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;


public enum ChaseState { Patrol, Chase, Wait, Retreat};
public class BaseEnemy : MonoBehaviour
{
    float hitLag;
    float swordHitFrame;

    public GameObject sword;
    //public GameObject center;

    Transform target;
    Vector3 basePosition;

    NavMeshAgent agent;

    [SerializeField] float radius;

    [SerializeField] RoomZoneTrigger baseRoom;

    ChaseState chaseState;

    float cooldownBeforeRetreat;
    [SerializeField] float timeBeforeRetreat;

    public Sprite Front, Back;
    [SerializeField] SpriteRenderer graphics;
    public Transform visuals;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        swordHitFrame = 0f;

        chaseState = ChaseState.Patrol;
        basePosition = transform.position;

        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        hitLag -= Time.deltaTime;


        if (hitLag > 0) return;


        bool isTargetInRadius = Vector2.Distance(transform.position, target.position) < radius;

        
        if (chaseState == ChaseState.Patrol)
        {
            if (baseRoom.isPlayerIn)
                chaseState = ChaseState.Chase;
        }

        if(chaseState == ChaseState.Chase) 
        {
            cooldownBeforeRetreat = timeBeforeRetreat;
            agent.SetDestination(new Vector3(target.position.x, target.position.y, transform.position.z));

            if (Vector2.Distance(transform.position, target.position) < 1.8f)
                Attack();

            if (!baseRoom.isPlayerIn && !isTargetInRadius)
                chaseState = ChaseState.Wait;

            if (Vector2.Distance(transform.position, target.position) < 1.4f)
                agent.SetDestination(transform.position);
        }

        if (chaseState == ChaseState.Wait)
        {
            cooldownBeforeRetreat -= Time.deltaTime;
            if (isTargetInRadius) chaseState = ChaseState.Chase;

            agent.SetDestination(transform.position);

            if (cooldownBeforeRetreat <= 0)
            {
                chaseState = ChaseState.Retreat;            
            }
        }

        if (chaseState == ChaseState.Retreat)
        {
            if (isTargetInRadius) chaseState = ChaseState.Chase;
            agent.SetDestination(basePosition);

            if (Vector2.Distance(basePosition, transform.position) < 2f)
            {
                chaseState = ChaseState.Patrol;
            }
        }


        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        Vector3 dir = agent.velocity;
        if (dir.y > 0.1f)
            graphics.sprite = Back;
        else if (dir.y < -0.1f)
            graphics.sprite = Front;

        if (dir.x > 0.1f)
            visuals.localScale = new Vector3(-1, 1, 1);
        if (dir.x < -0.1f)
            visuals.localScale = new Vector3(1, 1, 1);
    }

    void Attack()
    {
        if(hitLag > 0.1f) return;
        print("ATTACK");
        animator.SetTrigger("Attack");
        hitLag = 0.9f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    
}
