using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerObjectPool : MonoBehaviour
{
    public static CustomerObjectPool Instance { get; private set; }
    public GameObject prefab;
    public int poolSize = 20;
    public List<GameObject> ObjectPoolList { get; set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        ObjectPoolList = new();

        for(int i = 0; i < poolSize; i++)
        {
            var clone = Instantiate(prefab, transform);
            clone.SetActive(false);
            ObjectPoolList.Add(clone);
        }
    }

    public bool TryGetPoolObject(out GameObject poolObject)
    {
        poolObject = null;

        foreach(var item in ObjectPoolList)
        {
            if (!item.activeInHierarchy)
            {
                poolObject = item;
                return true;
            }
        }
        return false;
    }
}
