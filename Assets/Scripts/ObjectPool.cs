using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ObjectPoolEntry
{
    public GameObject prefab;
    public int poolSize;
    public List<GameObject> objectPoolList;
}

[Serializable]
public class ObjectToPoolDictionary : UnitySerializedDictionary<string, ObjectPoolEntry> { }

public class ObjectPool:MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    public ObjectToPoolDictionary ObjectToPoolDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        foreach(var objectToPool in ObjectToPoolDict)
        {
            var pool = new GameObject(objectToPool.Key);
            pool.transform.SetParent(transform);
            for(int i = 0; i < objectToPool.Value.poolSize; i++)
            {
                var clone = Instantiate(objectToPool.Value.prefab, pool.transform);
                clone.SetActive(false);
                objectToPool.Value.objectPoolList.Add(clone);
            }
        }

    }

    public bool TryGetPoolObject(string key,out GameObject poolObject)
    {
        poolObject = null;
        if (ObjectToPoolDict.TryGetValue(key, out ObjectPoolEntry objectPoolEntry))
        {
            foreach (var item in objectPoolEntry.objectPoolList)
            {
                if (!item.activeInHierarchy)
                {
                    poolObject = item;
                    return true;
                }
            }
        }

        return false;
    }
}