using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public abstract class TaskObject:BoardObject
{
    [OdinSerialize]
    [BoxGroup("user")]
    public Worker Worker { get; set; }  // when someone use it
    public bool IsAvailable { get => Worker == null; }
    public abstract Vector3Int WorkingCell { get; }
}

public class Bar : TaskObject
{
    TaskManager TaskManager { get => TaskManager.Instance; }
    public GameObject customerPrefab; //**** move to objectPool later
    public Transform spawnAt;
    public Transform exitAt;
    public Vector3Int SpawnCell { get => BoardManager.GetCellPos(spawnAt.position); }
    public Vector3Int ExitCell { get => BoardManager.GetCellPos(exitAt.position); }
    public Tilemap PathTile;

    // Customer
    [OdinSerialize] 
    [BoxGroup("user")]
    public Customer Customer { get; set; }
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

    public void CustomerLeave()
    {
        if (PathFinder.TryFindWaypoint(Customer, ServiceCell, ExitCell, Customer.dirs, out List<Vector3Int> waypoints))
        {
            Customer.CurrentState = new Customer.CustomerExitState(Customer, waypoints);
        }
        else
        {
            Debug.LogError($"can't move from {ServiceCell} to {ExitCell}");
        }
        Customer = null;
        SpawnNewCustomer();
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
    }

    public class ServeOrderTask : Task
    {
        public ServeOrderTask(Customer customer) : base(customer) 
        {
            performed += delegate { (TaskObject as Bar).CustomerLeave(); };
        }

        public override float Duration => 1f;

        public override bool TryGetTaskObject(Charecter charecter, out TaskObject taskObject)
        {
            taskObject = Customer.Bar;
            return true;
        }
    }
}
