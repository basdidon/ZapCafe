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
    public Vector3Int SpawnCell { get => BoardManager.GetCellPos(spawnAt.position); }
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
        var spawnPoint = BoardManager.GetCellCenterWorld(SpawnCell);

        var clone = Instantiate(customerPrefab,spawnPoint,Quaternion.identity);
        if(clone.TryGetComponent(out Customer customer))
        {
            customer.Initialized(this);
        }
        else
        {
            Debug.LogError("not found Customer's Script.");
        }
    }

    public void CustomerArrived(Customer customer)
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

        public bool CanExecute { get => true; }

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
