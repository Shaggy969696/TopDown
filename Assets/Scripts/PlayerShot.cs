using UnityEngine;
using UnityEngine.InputSystem;

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
    private bool isFiring = false;

    private void Start()
    {
        InitializeProjectilePool();
        ValidateReferences();
    }

    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    /// <summary>
    /// Llamado por el componente Player Input al realizar la acción de disparo.
    /// Configura el disparo continuo mientras se mantiene el botón pulsado.
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isFiring = true;
        }
        else if (context.canceled)
        {
            isFiring = false;
        }
    }

    /// <summary>
    /// Dispara un proyectil desde el firePoint
    /// </summary>
    private void Fire()
    {
        if (firePoint == null || projectilePool == null) return;

        GameObject projectileObj = projectilePool.GetObject();

        if (projectileObj == null)
        {
            Debug.LogError("[PlayerShot] No se pudo obtener proyectil del pool.");
            return;
        }

        projectileObj.transform.position = firePoint.position;
        projectileObj.transform.rotation = firePoint.rotation;

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetSpeed(projectileSpeed);
            projectile.Initialize(firePoint.forward, projectilePool);
        }
        else
        {
            Debug.LogError("[PlayerShot] El objeto del pool no tiene componente Projectile!");
        }
    }

    /// <summary>
    /// Inicializa el pool de proyectiles
    /// </summary>
    private void InitializeProjectilePool()
    {
        GameObject poolObject = new GameObject("ProjectilePool");
        poolObject.transform.SetParent(transform);

        projectilePool = poolObject.AddComponent<ObjectPool>();

        var poolType = typeof(ObjectPool);
        poolType.GetField("prefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(projectilePool, projectilePrefab);
        poolType.GetField("initialPoolSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(projectilePool, poolSize);
        poolType.GetField("autoExpand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(projectilePool, true);

        poolType.GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(projectilePool, null);
    }

    /// <summary>
    /// Valida que todas las referencias estén asignadas
    /// </summary>
    private void ValidateReferences()
    {
        if (firePoint == null)
            Debug.LogError("[PlayerShot] FirePoint no asignado! Asigna el Transform desde donde dispararás.");

        if (projectilePrefab == null)
            Debug.LogError("[PlayerShot] Projectile Prefab no asignado! Asigna el prefab del proyectil.");
        else if (projectilePrefab.GetComponent<Projectile>() == null)
            Debug.LogError("[PlayerShot] El prefab del proyectil NO tiene el componente 'Projectile' adjunto!");
    }

    /// <summary>
    /// Muestra el estado del pool en consola (para debugging)
    /// </summary>
    [ContextMenu("Show Pool Status")]
    public void ShowPoolStatus()
    {
        if (projectilePool != null)
            projectilePool.LogPoolStatus();
    }
}
