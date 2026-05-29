using UnityEngine;

public class PlayerAnima : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;

    private bool lastRunningState = false;
    private bool lastAttackingState = false;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogWarning("PlayerAnima: Animator no encontrado.");
        }
    }

    /// <summary>
    /// Activa o desactiva la animación de correr.
    /// </summary>
    public void SetRunning(bool isRunning)
    {
        if (animator == null) return;

        if (isRunning != lastRunningState)
        {
            lastRunningState = isRunning;
            Debug.Log($"[PlayerAnima] IsRunning: {isRunning}");
        }

        animator.SetBool("IsRunning", isRunning);
    }

    /// <summary>
    /// Activa o desactiva la animación de ataque.
    /// </summary>
    public void SetAttacking(bool isAttacking)
    {
        if (animator == null) return;

        if (isAttacking != lastAttackingState)
        {
            lastAttackingState = isAttacking;
            Debug.Log($"[PlayerAnima] IsAttacking: {isAttacking}");
        }

        animator.SetBool("IsAttacking", isAttacking);
    }

    /// <summary>
    /// Ajusta la velocidad de reproducción del Animator.
    /// 1.0 = velocidad normal, 2.0 = doble de rápido, 0.5 = mitad de velocidad.
    /// </summary>
    public void SetAttackSpeed(float speed)
    {
        if (animator == null) return;
        animator.speed = speed;
    }
}
