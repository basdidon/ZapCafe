using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public abstract class TaskObject:BoardObject
{
    [OdinSerialize] public Worker Worker { get; set; }
    public bool IsAvailable { get => Worker == null; }
    public abstract Vector3Int WorkingCell { get; }
}

public class Bar : TaskObject
{
    TaskManager TaskManager { get => TaskManager.Instance; }
    public GameObject customerPrefab; //**** move to objectPool later
    public Transform spawnAt;
    public Vector3Int SpawnCell { get => BoardManager.GetCellPos(spawnAt.position); }
    public Tilemap PathTile;

    // Customer
    [field: SerializeField] public Transform ServicePoint { get; set; }
    public Vector3Int ServiceCell { get => BoardManager.GetCellPos(ServicePoint.position); }

    // Worker
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public override Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    // Mono
    private void Start()
    {
        TaskManager.AddTaskObject(this);
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
        TaskManager.AddTask(new GetOrderTask(customer,this));
    }

    public class GetOrderTask : Task
    {
        public Bar Bar { get; set; }
        public override float Duration => 5f;

        public GetOrderTask(Customer customer,Bar bar):base(customer)
        {
            Bar = bar;
        }

        public override bool TryGetTaskObject(Charecter charecter, out TaskObject taskObject)
        {
            taskObject = Bar;
            return true;
        }

        public override Task Execute()
        {
            Debug.Log("Task execute()");
            Customer.GetOrder();
            return null;
        }
    }

    public class ServeOrderTask : Task
    {
        public ServeOrderTask(Customer customer) : base(customer) { }

        public override float Duration => 0f;

        public override Task Execute()
        {
            Debug.Log("order served");
            return null;
        }

        public override bool TryGetTaskObject(Charecter charecter, out TaskObject taskObject)
        {
            taskObject = Customer.Bar;
            return true;
        }
    }
}
