using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class Customer : Charecter
{
    [field: SerializeField] public Tilemap PathTilemap { get; set; }
    [field: SerializeField] public Bar Bar { get; set; }
    
    public void Initialized(Bar bar)
    {
        Bar = bar;
        PathTilemap = Bar.PathTile;

        var dirs = new List<Vector3Int>() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        if (PathFinder.TryFindWaypoint(this, Bar.SpawnCell, Bar.ServiceCell, dirs, out List<Vector3Int> waypoints))
        {
            CurrentState = new CustomerMoveState(this, waypoints);
        }
        else
        {
            Debug.LogError($"can't move from {Bar.SpawnCell} to {Bar.ServiceCell}");
        }
    }

    // Monobehaviour
    protected void Awake()
    {
        IdleState = new CustomerIdleState();
    }

    public override bool CanMoveTo(Vector3Int cellPos) => PathTilemap.HasTile(cellPos);

    public void GetOrder()
    {
        Debug.Log("getOrder");
        var a = new GetDonut(this);
        TaskManager.Instance.AddTask(new GetDonut(this));
    }

    #region State
    public class CustomerIdleState : IState
    {
        public void EnterState(){}
        public void ExitState(){}
    }

    public class CustomerMoveState : MoveState<Customer>
    {
        public CustomerMoveState(Customer charecter,List<Vector3Int> waypoints):base(charecter,waypoints){}

        public override void SetNextState()
        {
            if (WayPoints.Count == 0)
            {
                if (Charecter.CellPosition == Charecter.Bar.ServiceCell)
                {
                    //Charecter.Bar.CustomerArrived(Charecter);
                    Charecter.Bar.Customer = Charecter;
                    var newTask = new Bar.GetOrderTask(Charecter, Charecter.Bar);
                    newTask.performed += Charecter.GetOrder;
                    TaskManager.Instance.AddTask(newTask);
                }

                Charecter.CurrentState = Charecter.IdleState;
            }
            else
            {
                // self transition
                Charecter.CurrentState = new CustomerMoveState(Charecter,WayPoints);
            }
        }
    }
    #endregion
}


