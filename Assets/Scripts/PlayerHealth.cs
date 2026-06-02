using UnityEngine;

/// <summary>
/// Gestiona la vida del jugador. Implementa IDamageable para recibir daño
/// desde cualquier fuente (enemigos, proyectiles, etc).
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 10f;

    [Header("Invencibilidad tras recibir daño")]
    [SerializeField] private float invincibilityDuration = 0.5f;

    private float currentHealth;
    private float lastDamageTime = -Mathf.Infinity;

    public float CurrentHealth => currentHealth;
    public float MaxHealth     => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UIManager.Instance?.InitHealthBar(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (Time.time < lastDamageTime + invincibilityDuration) return;

        lastDamageTime = Time.time;
        currentHealth  = Mathf.Max(currentHealth - amount, 0f);

        UIManager.Instance?.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UIManager.Instance?.UpdateHealth(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] El jugador ha muerto.");
        UIManager.Instance?.ShowEndPanel("DERROTA");
    }
}