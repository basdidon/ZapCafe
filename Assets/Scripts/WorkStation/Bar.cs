using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterState.CustomerState;
using BasDidon.Direction;
using BasDidon.PathFinder;

public class Bar : WorkStation
{
    public Transform exitAt;
    public Vector3Int ExitCell { get => BoardManager.GetCellPos(exitAt.position); }

    // Customer
    [field: SerializeField] public Customer Customer { get; set; }
    [field: SerializeField] public Vector3Int ServiceCellLocal { get; private set; }
    public Vector3Int ServiceCell => CellPosition + ServiceCellLocal;

    public void CustomerLeave()
    {
        if (Customer == null)
            Debug.Log("Customer null");

        if (GridPathFinder.TryFindPath(Customer, ServiceCell, ExitCell, DirectionGroup.Cardinal, out PathTraced pathTraced))
        {
            Customer.CurrentState = new MoveToExitState(Customer, pathTraced.ToWayPoint());
        }
        else
        {
            Debug.LogError($"can't move from {ServiceCell} to {ExitCell}");
        }

        Customer = null;
    }

    
}
