using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Se registra como enemigo activo al aparecer en escena
    private void OnEnable()
    {
        UIManager.Instance?.RegisterEnemy();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        Debug.Log($"[EnemyHealth] {gameObject.name} eliminado.");
        UIManager.Instance?.RegisterEnemyKill();
        gameObject.SetActive(false);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}