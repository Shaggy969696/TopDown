using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Clase base para enemigos. Maneja la detección y persecución usando NavMeshAgent.
/// Preparada para ser heredada por tipos específicos de enemigos y para integrar animaciones.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    [Header("Detección Base")]
    [SerializeField] protected float detectionRadius = 10f;

    // Componentes y referencias protegidas para que las clases hijas puedan acceder
    protected Transform player;
    protected NavMeshAgent agent;
    protected bool isChasing;

    // Referencia preparada para el futuro
    // protected Animator animator;

    protected virtual void Awake()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.transform;
        }

        agent = GetComponent<NavMeshAgent>();

        // Cuando tengas el modelo animado:
        // animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        CheckDetection();
        HandleMovement();
        HandleAnimations();
    }

    // Comprueba si el jugador entró en el radio
    protected virtual void CheckDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isChasing = distanceToPlayer <= detectionRadius;
    }

    // Lógica principal de movimiento del NavMesh
    protected virtual void HandleMovement()
    {
        if (isChasing)
        {
            agent.SetDestination(player.position);

            // Llegamos a la distancia de frenado
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
                Attack();
            }
        }
        else
        {
            // Si el jugador se aleja, nos detenemos o patrullamos
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
            Patrol();
        }
    }

    // Rota hacia el jugador ignorando el eje Y (para no inclinarse hacia arriba/abajo)
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

    // Métodos vacíos listos para ser sobrescritos (override) en scripts hijos
    protected virtual void Attack()
    {
        // Acá irá la lógica de daño, disparos, etc.
    }

    protected virtual void Patrol()
    {
        // Acá podrías decirle al agente que vaya a puntos aleatorios si no está persiguiendo
    }

    // Método listo para cuando agregues el componente Animator
    protected virtual void HandleAnimations()
    {
        /*
        if (animator != null)
        {
            // El NavMeshAgent calcula automáticamente a qué velocidad se está moviendo.
            // Le pasamos ese valor al Animator para transicionar entre Idle, Caminar y Correr.
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }
        */
    }

    // Dibuja el radio de detección en la escena para depurar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}