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
        // Auto-retornar al pool despu�s del tiempo de vida
        if (Time.time - spawnTime >= lifetime)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// Inicializa el proyectil con direcci�n y referencia al pool
    /// </summary>
    public void Initialize(Vector3 direction, ObjectPool pool)
    {
        parentPool = pool;
        rb.linearVelocity = direction.normalized * speed;
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
        // Ignorar colisiones con objetos tagged como "Player" si viene del player
        if (hitObject.CompareTag("Player"))
        {
            return;
        }

        if (hitObject.CompareTag("Enemy"))
        {
            Debug.Log($"[PROYECTIL] Impact� enemigo: {hitObject.name}");
            // Aqu� puedes agregar l�gica de da�o: hitObject.GetComponent<Enemy>()?.TakeDamage(damage);
        }
        else
        {
            Debug.Log($"[PROYECTIL] Colision� con: {hitObject.name}");
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
            parentPool.ReturnObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}