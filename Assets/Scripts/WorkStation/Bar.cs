using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Bar : BoardObject,IWorkStation
{
    public Transform exitAt;
    public Vector3Int ExitCell { get => BoardManager.GetCellPos(exitAt.position); }

    // Customer
    [OdinSerialize] 
    [BoxGroup("user")]
    [field:SerializeField] public Customer Customer { get; set; }
    [field: SerializeField] public Transform ServicePoint { get; set; }
    public Vector3Int ServiceCell { get => BoardManager.GetCellPos(ServicePoint.position); }

    // Worker
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }
    [field:SerializeField] public WorkStationData WorkStationData { get; private set; }

    // Mono
    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
        TaskManager.Instance.WorkStationFree();
    }

    public void CustomerLeave()
    {
        if (Customer == null)
            Debug.Log("Customer null");

        if (PathFinder.TryFindWaypoint(Customer, ServiceCell, ExitCell, Customer.dirs, out List<Vector3Int> waypoints))
        {
            Customer.CurrentState = new Customer.CustomerExitState(Customer, waypoints);
        }
        else
        {
            Debug.LogError($"can't move from {ServiceCell} to {ExitCell}");
        }

        Customer = null;
    }

    
}
