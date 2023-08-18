using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : WorkStation
{
    public Transform exitAt;
    public Vector3Int ExitCell { get => BoardManager.GetCellPos(exitAt.position); }

    // Customer
    [field: SerializeField] public Customer Customer { get; set; }
    [field: SerializeField] public Vector3Int ServiceCellLocal { get; private set; }
    public Vector3Int ServiceCell => CellPosition + ServiceCellLocal;

    // Mono
    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
        TaskManager.Instance.TrySetTask();
    }

    public void CustomerLeave()
    {
        if (Customer == null)
            Debug.Log("Customer null");

        if (PathFinder.TryFindWaypoint(Customer, ServiceCell, ExitCell, Customer.dirs, out List<Vector3Int> waypoints))
        {
            Customer.CurrentState = new CustomerState.MoveToExitState(Customer, waypoints);
        }
        else
        {
            Debug.LogError($"can't move from {ServiceCell} to {ExitCell}");
        }

        Customer = null;
    }

    
}
