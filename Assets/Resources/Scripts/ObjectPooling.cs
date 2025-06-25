using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooling : MonoBehaviour
{
    
    public static ObjectPooling Instance;
   // public int defaultPoolSize = 10;
    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int poolSize = 10;
    }
    public List<PoolEntry> poolSettings;

    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitializePools();
    }
    private void InitializePools()
    {
        foreach (PoolEntry entry in poolSettings)
        {
            GameObject prefab = entry.prefab;
            int size = entry.poolSize;

            if (prefab == null) continue;

            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            pools[prefab] = pool;
        }
    }

    public GameObject GetFromPool(GameObject prefab, Vector2 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new Queue<GameObject>();
            int size = poolSettings.Find(p => p.prefab == prefab)?.poolSize ?? 15;
            ExpandPool(prefab, size);
        }

        GameObject obj;

        if (pools[prefab].Count > 0)
        {
            obj = pools[prefab].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        var pooledObj = obj.GetComponent<PooledObject>();
        if (pooledObj == null)
        {
            pooledObj = obj.AddComponent<PooledObject>();
        }
        pooledObj.prefab = prefab;

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        var pooledObj = obj.GetComponent<PooledObject>();
        if (pooledObj == null || pooledObj.prefab == null)
        {
            Debug.LogWarning("Object không có PooledObject hoặc prefab gốc.");
            Destroy(obj); // fallback nếu không có prefab để biết đường trả lại
            return;
        }

        obj.SetActive(false);

        if (!pools.ContainsKey(pooledObj.prefab))
        {
            pools[pooledObj.prefab] = new Queue<GameObject>();
        }

        pools[pooledObj.prefab].Enqueue(obj);
    }

    private void ExpandPool(GameObject prefab, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pools[prefab].Enqueue(obj);
        }
    }
}
