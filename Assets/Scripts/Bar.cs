using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Bar : BoardObject,IWorkStation<Item>
{
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
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    // Mono
    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
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
        Customer.OrderSprite = null;
        Customer = null;
        SpawnNewCustomer();
    }

    public Item GetItem() => throw new System.NotImplementedException();
    public Sprite Sprite { get => throw new System.NotImplementedException(); }

    public class GetOrderTask : Task<Item>
    {
        public Bar Bar { get; set; }
        public override float Duration => 5f;

        public GetOrderTask(Customer customer,Bar bar):base(customer)
        {
            Bar = bar;
        }
        public override IWorkStation<Item> GetworkStation(Worker worker)
        {
            return Bar;
        }
    }

    public class ServeOrderTask : Task<Item>
    {
        public ServeOrderTask(Customer customer) : base(customer) 
        {
            Performed += delegate { (WorkStation as Bar).CustomerLeave(); };
        }

        public override float Duration => 1f;

        public override IWorkStation<Item> GetworkStation(Worker worker)
        {
            foreach(Bar bar in WorkStationRegistry.Instance.GetWorkStationsByType<Bar>())
            {
                if(bar.Customer == Customer)
                {
                    return bar;
                }
            }

            return null;
        }
    }
}
