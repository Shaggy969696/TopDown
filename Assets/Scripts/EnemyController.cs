using UnityEngine;

/// <summary>
/// Enemigo que detecta y persigue al player dentro de una distancia.
/// Funciona con CharacterController si está presente; si no, mueve el transform directamente.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Detección y persecución")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Opciones")]
    [Tooltip("Si está marcado, intentará usar CharacterController si existe en el GameObject.")]
    [SerializeField] private bool preferCharacterController = true;

    private Transform player;
    private CharacterController characterController;
    private bool isChasing;

    private void Awake()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.transform;
        }
        else
        {
            Debug.LogWarning("[EnemyController] No se encontró ningún GameObject con tag 'Player'.");
        }

        if (preferCharacterController)
        {
            characterController = GetComponent<CharacterController>();
            // Si no existe, no forzamos su creación — permitimos movimiento por transform.
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f; // ignorar altura para detección 2D/top-down

        float sqrDist = toPlayer.sqrMagnitude;
        float detectionSqr = detectionRadius * detectionRadius;
        float stoppingSqr = stoppingDistance * stoppingDistance;

        isChasing = sqrDist <= detectionSqr;

        if (!isChasing)
        {
            // Aquí podrías añadir comportamiento de patrulla o idle
            return;
        }

        // Rotación suave hacia el jugador (solo en Y)
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Movimiento: solo si estamos fuera de la distancia de parada
        if (sqrDist > stoppingSqr)
        {
            Vector3 moveDir = toPlayer.normalized;

            if (characterController != null)
            {
                // CharacterController no aplica gravedad automáticamente para este caso top-down.
                characterController.Move(moveDir * chaseSpeed * Time.deltaTime);
            }
            else
            {
                // Movimiento transform directo (simple y apropiado para IA básica)
                transform.position += moveDir * chaseSpeed * Time.deltaTime;
            }
        }
        else
        {
            // Dentro del rango de parada -> quedarnos o atacar
            // Aquí puedes invocar una rutina de ataque o animación
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualización en escena
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}
