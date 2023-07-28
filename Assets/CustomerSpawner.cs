using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomerSpawner : MonoBehaviour
{
    public Transform spawnAt;
    public Vector3Int SpawnCell { get => BoardManager.Instance.GetCellPos(spawnAt.position); }
    public Tilemap PathTile;

    public float minWaitTime;
    public float maxWaitTime;
    public float waitTime;

    public float TimeElapsed { get; set; }

    private void Start()
    {
        SpawnCustomer();
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        TimeElapsed = 0;
    }

    private void Update()
    {
        if(TimeElapsed >= waitTime)
        {
            SpawnCustomer();
            waitTime = Random.Range(minWaitTime, maxWaitTime);
            TimeElapsed = 0;
        }
        else
        {
            TimeElapsed += Time.deltaTime;
        }
    }

    private void SpawnCustomer()
    {
        List<Bar> availableBar = new();
        foreach (Bar bar in WorkStationRegistry.Instance.GetWorkStationsByType<Bar>())
            if (bar.Customer == null)
                availableBar.Add(bar);

        if (availableBar.Count > 0)
        {
            //spawn cust
            if (ObjectPool.Instance.TryGetPoolObject("Customer", out GameObject customerGO))
            {
                customerGO.SetActive(true);
                customerGO.transform.position = BoardManager.Instance.GetCellCenterWorld(SpawnCell);
                if (customerGO.TryGetComponent(out Customer customer))
                {
                    customer.Initialized(availableBar[Random.Range(0, availableBar.Count)], PathTile);
                }
                else
                {
                    Debug.LogError("not found Customer's Script.");
                }
            }
        }
    }
}
