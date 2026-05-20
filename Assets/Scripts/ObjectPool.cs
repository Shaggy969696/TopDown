using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema genérico de Object Pooling para optimizar instanciación de GameObjects
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("Pool Configuration")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private bool autoExpand = true;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();

    private void Start()
    {
        InitializePool();
    }

    /// <summary>
    /// Inicializa el pool con objetos desactivados
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// Crea un nuevo objeto y lo añade al pool
    /// </summary>
    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// Obtiene un objeto del pool
    /// </summary>
    public GameObject GetObject()
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else if (autoExpand)
        {
            Debug.LogWarning($"Pool vacío, expandiendo automáticamente. Considera aumentar el tamaño inicial.");
            obj = CreateNewObject();
        }
        else
        {
            Debug.LogError("Pool vacío y auto-expansión desactivada.");
            return null;
        }

        obj.SetActive(true);
        activeObjects.Add(obj);
        return obj;
    }

    /// <summary>
    /// Devuelve un objeto al pool
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        activeObjects.Remove(obj);
        pool.Enqueue(obj);
    }

    /// <summary>
    /// Devuelve todos los objetos activos al pool
    /// </summary>
    public void ReturnAllObjects()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            ReturnObject(activeObjects[i]);
        }
    }

    /// <summary>
    /// Información del estado del pool
    /// </summary>
    public void LogPoolStatus()
    {
        Debug.Log($"Pool Status - Disponibles: {pool.Count} | Activos: {activeObjects.Count} | Total: {pool.Count + activeObjects.Count}");
    }
}