using UnityEngine;

/// <summary>
/// Controlador de disparo del jugador usando Object Pooling
/// </summary>
public class PlayerShot : MonoBehaviour
{
    [Header("Shooting Configuration")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 20;

    private ObjectPool projectilePool;
    private float nextFireTime = 0f;

    private void Start()
    {
        InitializeProjectilePool();
        ValidateReferences();
    }

    private void Update()
    {
        HandleShootInput();
    }

    /// <summary>
    /// Inicializa el pool de proyectiles
    /// </summary>
    private void InitializeProjectilePool()
    {
        GameObject poolObject = new GameObject("ProjectilePool");
        poolObject.transform.SetParent(transform);
        
        projectilePool = poolObject.AddComponent<ObjectPool>();
        
        // Configurar el pool mediante reflection ya que los campos son privados
        var poolType = typeof(ObjectPool);
        poolType.GetField("prefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(projectilePool, projectilePrefab);
        poolType.GetField("initialPoolSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(projectilePool, poolSize);
        poolType.GetField("autoExpand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(projectilePool, true);

        // Forzar inicialización
        poolType.GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(projectilePool, null);
    }

    /// <summary>
    /// Valida que todas las referencias estén asignadas
    /// </summary>
    private void ValidateReferences()
    {
        if (firePoint == null)
        {
            Debug.LogError("[PlayerShot] FirePoint no asignado! Asigna el Transform desde donde dispararás.");
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("[PlayerShot] Projectile Prefab no asignado! Asigna el prefab del proyectil.");
        }
        else if (projectilePrefab.GetComponent<Projectile>() == null)
        {
            Debug.LogError("[PlayerShot] El prefab del proyectil NO tiene el componente 'Projectile' adjunto!");
        }
    }

    /// <summary>
    /// Maneja el input del jugador para disparar
    /// </summary>
    private void HandleShootInput()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    /// <summary>
    /// Dispara un proyectil desde el firePoint
    /// </summary>
    private void Shoot()
    {
        if (firePoint == null || projectilePool == null) return;

        GameObject projectileObj = projectilePool.GetObject();
        
        if (projectileObj == null)
        {
            Debug.LogError("[PlayerShot] No se pudo obtener proyectil del pool.");
            return;
        }

        // Posicionar el proyectil en el firePoint
        projectileObj.transform.position = firePoint.position;
        projectileObj.transform.rotation = firePoint.rotation;

        // Inicializar el proyectil con dirección y pool
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(firePoint.forward, projectilePool);
            projectile.SetSpeed(projectileSpeed);
        }
        else
        {
            Debug.LogError("[PlayerShot] El objeto del pool no tiene componente Projectile!");
        }
    }

    /// <summary>
    /// Muestra el estado del pool en consola (para debugging)
    /// </summary>
    [ContextMenu("Show Pool Status")]
    public void ShowPoolStatus()
    {
        if (projectilePool != null)
        {
            projectilePool.LogPoolStatus();
        }
    }
}
