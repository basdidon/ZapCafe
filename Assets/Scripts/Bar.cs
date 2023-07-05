using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class Bar : BoardObject
{
    TaskManager TaskManager { get => TaskManager.Instance; }
    public GameObject customerPrefab; //**** move to objectPool later
    public Transform spawnAt;
    public Tilemap PathTile;

    // Customer
    [field: SerializeField] public Customer Customer { get; set; }
    [field: SerializeField] public Transform ServicePoint { get; set; }
    public Vector3Int ServiceCell { get => BoardManager.GetCellPos(ServicePoint.position); }

    // Worker
    [field: SerializeField] public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }

    // Events

    // Mono
    private void Start()
    {
        SpawnNewCustomer();
    }

    // Method
    [Button]
    public void SpawnNewCustomer()
    {
        var spawnCell = BoardManager.GetCellPos(spawnAt.position);
        var spawnPoint = BoardManager.GetCellCenterWorld(spawnCell);
        var clone = Instantiate(customerPrefab,spawnPoint,Quaternion.identity);
        if(clone.TryGetComponent(out Customer customer))
        {
            customer.PathTilemap = PathTile;
            var dirs = new List<Vector3Int>() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
            if (PathFinder.TryFindWaypoint(customer, spawnCell, ServiceCell, dirs,out List<Vector3Int> waypoints))
            {
                customer.WayPoints = waypoints;
            }
            else
            {
                Debug.LogError($"can't move from {spawnCell} to {ServiceCell}");
            }
        }
        else
        {
            Debug.LogError("not found Customer's Script.");
        }
    }

    public void AddNewCustomer(Customer customer)
    {
        Customer = customer;
        TaskManager.AddTasks(new GetOrderTask(this));
    }

    public class GetOrderTask : ITask
    {
        Bar Bar { get; } 
        public GetOrderTask(Bar bar)
        {
            Bar = bar;
        }

        public void Execute()
        {
            //Bar.StartCoroutine(GetOrder());
        }
        /*
        IEnumerator GetOrder()
        {

        }*/
    }

}
