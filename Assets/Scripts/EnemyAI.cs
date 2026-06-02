using UnityEngine;

/// <summary>
/// Enemigo de contacto con tres estados claros: Idle → Chase → Attack.
/// </summary>
public class EnemyAI : EnemyBase
{
    [Header("Rangos")]
    [SerializeField] private float chaseRange  = 10f;  // distancia a la que empieza a perseguir
    [SerializeField] private float attackRange = 1.5f; // distancia a la que se detiene y ataca

    [Header("Ataque")]
    [SerializeField] private float contactDamage  = 10f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float attackCooldown = 1f;

    private enum State { Idle, Chase, Attack }
    private State currentState;

    private PlayerController playerController;
    private IDamageable playerDamageable;
    private float lastAttackTime = -Mathf.Infinity;

    protected override void Awake()
    {
        base.Awake();
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerDamageable = player.GetComponent<IDamageable>();
        }
        // EnemyBase no controlará el movimiento: lo manejamos aquí completamente
        agent.stoppingDistance = 0f;
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

        if      (distance <= attackRange) currentState = State.Attack;
        else if (distance <= chaseRange)  currentState = State.Chase;
        else                              currentState = State.Idle;
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                agent.isStopped = true;
                agent.velocity  = Vector3.zero;
                break;

            case State.Chase:
                agent.isStopped = false;
                agent.SetDestination(player.position);
                break;

            case State.Attack:
                // Detenerse justo donde está y atacar
                agent.isStopped = true;
                agent.velocity  = Vector3.zero;
                FaceTarget();
                TryAttack();
                break;
        }
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        playerDamageable?.TakeDamage(contactDamage);

        Vector3 knockDir = (player.position - transform.position);
        knockDir.y = 0f;
        playerController?.ApplyKnockback(knockDir, knockbackForce);

        Debug.Log($"[EnemyAI] Atacó al jugador. Daño: {contactDamage}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);   // persecución
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);  // ataque
    }
}