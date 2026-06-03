using System.Reflection;
using System.Collections;
using UnityEngine;

/// <summary>
/// Componente que convierte un GameObject en un punto de spawn con su propio ObjectPool.
/// Permite spawn automático cada cierto tiempo y asignar el prefab por punto desde el inspector.
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Pool / Enemy")]
    [Tooltip("Prefab del enemigo que se instanciará desde este punto")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private bool autoExpand = true;

    [Header("Spawning")]
    [Tooltip("Tiempo entre spawns en segundos")]
    [SerializeField] private float spawnInterval = 5f;
    [Tooltip("Retardo inicial antes del primer spawn")]
    [SerializeField] private float initialDelay = 0f;
    [Tooltip("Si está activo, el punto comenzará a spawnear al iniciar")]
    [SerializeField] private bool spawnOnStart = true;

    private ObjectPool pool;
    private Coroutine spawnRoutine;

    private void Start()
    {
        if (enemyPrefab != null)
        {
            EnsurePool();
            if (spawnOnStart)
                StartSpawning();
        }
    }

    public void EnsurePool()
    {
        if (pool != null) return;

        GameObject poolObject = new GameObject($"Pool_{name}");
        poolObject.transform.SetParent(transform);
        pool = poolObject.AddComponent<ObjectPool>();

        var poolType = typeof(ObjectPool);
        poolType.GetField("prefab", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(pool, enemyPrefab);
        poolType.GetField("initialPoolSize", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(pool, poolSize);
        poolType.GetField("autoExpand", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(pool, autoExpand);

        // Forzar inicialización si Start es privado
        poolType.GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(pool, null);
    }

    /// <summary>
    /// Obtiene un enemigo del pool y lo posiciona en este spawn point.
    /// También aplica el multiplicador de detección si está configurado.
    /// También aplica el multiplicador de detección si está configurado.
    /// También aplica el multiplicador de detección si está configurado.
    /// También aplica el multiplicador de detección si está configurado.
        /// </summary>
    public GameObject Spawn()
    {
        EnsurePool();

        if (pool == null) return null;

        GameObject go = pool.GetObject();
        if (go == null) return null;

        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.SetActive(true);

        return go;
    }

    /// <summary>
    /// Inicia el spawn automático en intervalos.
    /// </summary>
    public void StartSpawning()
    {
        EnsurePool();
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(AutoSpawnCoroutine());
    }

    /// <summary>
    /// Para el spawn automático.
    /// </summary>
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator AutoSpawnCoroutine()
    {
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(Mathf.Max(0.01f, spawnInterval));
        }
    }

    /// <summary>
    /// Permite asignar/actualizar el prefab desde código o UI.
    /// </summary>
    public void SetEnemyPrefab(GameObject prefab)
    {
        enemyPrefab = prefab;
        if (pool != null)
        {
            var poolType = typeof(ObjectPool);
            poolType.GetField("prefab", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(pool, enemyPrefab);
            poolType.GetMethod("ReturnAllObjects", BindingFlags.Public | BindingFlags.Instance)
                ?.Invoke(pool, null);
        }
    }

    [ContextMenu("CreatePoolNow")]
    private void CreatePoolNow() => EnsurePool();

    [ContextMenu("SpawnNow")]
    private void SpawnNow() => Spawn();

    [ContextMenu("StartSpawningNow")]
    private void StartSpawningNow() => StartSpawning();

    [ContextMenu("StopSpawningNow")]
    private void StopSpawningNow() => StopSpawning();
}