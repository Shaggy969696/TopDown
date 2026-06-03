using UnityEngine;

/// <summary>
/// Enemigo de contacto con tres estados claros: Idle → Chase → Attack.
/// </summary>
public class EnemyAI : EnemyBase
{
    [Header("Rangos")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Ataque")]
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float attackCooldown = 1f;

    private enum State { Idle, Chase, Attack }
    private State currentState;

    private PlayerController playerController;
    private IDamageable playerDamageable;
    private Animator anim;

    private float lastAttackTime = -Mathf.Infinity;

    protected override void Awake()
    {
        base.Awake();

        // Busca Animator en este objeto o en los hijos
        anim = GetComponentInChildren<Animator>();

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerDamageable = player.GetComponent<IDamageable>();
        }

        agent.stoppingDistance = 0f;

        if (anim == null)
        {
            Debug.LogError("EnemyAI: No se encontró Animator.");
        }
    }

    protected override void Update()
    {
        if (player == null) return;

        UpdateState();
        HandleState();
        HandleAnimations();
    }

    private void UpdateState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
            currentState = State.Attack;
        else if (distance <= chaseRange)
            currentState = State.Chase;
        else
            currentState = State.Idle;
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                break;

            case State.Chase:
                agent.isStopped = false;
                agent.SetDestination(player.position);
                break;

            case State.Attack:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                FaceTarget();
                TryAttack();
                break;
        }
    }

    private void HandleAnimations()
    {
        if (anim == null) return;

        float speed = agent.velocity.magnitude;

        // Parámetros del Animator del tigre
        anim.SetFloat("Vert", Mathf.Clamp01(speed));
        anim.SetFloat("State", 1f);
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        playerDamageable?.TakeDamage(contactDamage);

        Vector3 knockDir = player.position - transform.position;
        knockDir.y = 0f;

        playerController?.ApplyKnockback(knockDir, knockbackForce);

        Debug.Log($"[EnemyAI] Atacó al jugador. Daño: {contactDamage}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}