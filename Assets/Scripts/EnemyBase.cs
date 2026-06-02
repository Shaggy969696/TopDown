using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    [Header("Detección Base")]
    [SerializeField] protected float detectionRadius = 10f;

    protected Transform player;
    protected NavMeshAgent agent;
    protected bool isChasing;

    protected virtual void Awake()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null) player = playerGo.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if (player == null) return;
        CheckDetection();
        HandleMovement();
        HandleAnimations();
    }

    protected virtual void CheckDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isChasing = distanceToPlayer <= detectionRadius;
    }

    protected virtual void HandleMovement()
    {
        if (isChasing)
        {
            bool inAttackRange = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

            if (inAttackRange)
            {
                // Detener completamente el agente para que no empuje al player
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                FaceTarget();
                Attack();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            agent.isStopped = false;
            if (agent.hasPath) agent.ResetPath();
            Patrol();
        }
    }

    protected virtual void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed / 50f);
        }
    }

    protected virtual void Attack() { }
    protected virtual void Patrol() { }
    protected virtual void HandleAnimations() { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}