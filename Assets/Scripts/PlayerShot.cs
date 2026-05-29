using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador de disparo del jugador usando Object Pooling.
/// La velocidad de la animación de ataque se sincroniza automáticamente con el fireRate.
/// </summary>
public class PlayerShot : MonoBehaviour
{
    [Header("Shooting Configuration")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 25f;

    [Header("Attack Sync")]
    [Tooltip("Tiempo en segundos entre cada disparo. Modifica SOLO este valor, la animación se adapta.")]
    [SerializeField] private float fireRate = 0.8f;

    [Tooltip("Duración original del clip attack01 en segundos (no tocar).")]
    [SerializeField] private float attackClipDuration = 1.333f;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 20;

    private ObjectPool projectilePool;
    private PlayerAnima playerAnima;
    private float nextFireTime = 0f;
    private bool isFiring = false;

    // Velocidad calculada: cuántas veces más rápido debe correr la animación
    private float AttackAnimSpeed => attackClipDuration / fireRate;

    private void Start()
    {
        InitializeProjectilePool();
        ValidateReferences();

        playerAnima = GetComponentInChildren<PlayerAnima>();
        if (playerAnima == null)
            Debug.LogWarning("[PlayerShot] No se encontró PlayerAnima en los hijos.");
    }

    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

        playerAnima?.SetAttacking(isFiring);

        // Ajusta la velocidad del animator: rápido al atacar, normal al idle/run
        playerAnima?.SetAttackSpeed(isFiring ? AttackAnimSpeed : 1f);
    }

    /// <summary>
    /// Llamado por el Player Input al hacer clic izquierdo.
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log($"[PlayerShot] OnFire llamado - phase: {context.phase}");

        if (context.started)
        {
            isFiring = true;
            Debug.Log("[PlayerShot] isFiring = TRUE");
        }
        else if (context.canceled)
        {
            isFiring = false;
            Debug.Log("[PlayerShot] isFiring = FALSE");
        }
    }

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

    private void ValidateReferences()
    {
        if (firePoint == null)
            Debug.LogError("[PlayerShot] FirePoint no asignado!");
        if (projectilePrefab == null)
            Debug.LogError("[PlayerShot] Projectile Prefab no asignado!");
        else if (projectilePrefab.GetComponent<Projectile>() == null)
            Debug.LogError("[PlayerShot] El prefab no tiene el componente Projectile!");
    }

    [ContextMenu("Show Pool Status")]
    public void ShowPoolStatus() => projectilePool?.LogPoolStatus();
}
