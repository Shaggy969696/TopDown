using UnityEngine;

/// <summary>
/// Comportamiento de proyectil que se auto-retorna al pool
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 25f;

    private Rigidbody rb;
    private ObjectPool parentPool;
    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - spawnTime >= lifetime)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// Inicializa el proyectil con dirección y referencia al pool
    /// </summary>
    public void Initialize(Vector3 direction, ObjectPool pool)
    {
        parentPool = pool;
        transform.SetParent(null);
        spawnTime = Time.time;
        rb.linearVelocity = direction.normalized * speed;
        rb.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Configura la velocidad del proyectil
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    /// <summary>
    /// Maneja las colisiones del proyectil
    /// </summary>
    private void HandleCollision(GameObject hitObject)
    {
        // Ignorar colisiones con el propio jugador
        if (hitObject.CompareTag("Player"))
            return;

        // Buscar IDamageable subiendo por la jerarquía (el collider puede estar en un hijo)
        IDamageable damageable = hitObject.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            Debug.Log($"[PROYECTIL] Impactó: {hitObject.transform.root.name}");
            damageable.TakeDamage(damage);
        }
        else
        {
            Debug.Log($"[PROYECTIL] Colisionó con: {hitObject.name} (sin IDamageable)");
        }

        ReturnToPool();
    }

    /// <summary>
    /// Retorna el proyectil al pool
    /// </summary>
    private void ReturnToPool()
    {
        if (parentPool != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.SetParent(parentPool.transform);
            parentPool.ReturnObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}