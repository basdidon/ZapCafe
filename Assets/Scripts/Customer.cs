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
            WayPoints = waypoints;
        }
        else
        {
            Debug.LogError($"can't move from {Bar.SpawnCell} to {Bar.ServiceCell}");
        }
    }

    // Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        IdleState = new CustomerIdleState();
        MoveState = new CustomerMoveState(this);
    }

    public override bool CanMoveTo(Vector3Int cellPos) => PathTilemap.HasTile(cellPos);

    public void GetOrder()
    {
        TaskManager.Instance.AddTask(new DonutBox.GetDonut(this));
    }

    #region State
    public class CustomerIdleState : IState
    {
        public void EnterState(){}
        public void ExitState(){}
    }

    public class CustomerMoveState : MoveState<Customer>
    {
        public CustomerMoveState(Customer charecter):base(charecter){}

        public override void SetNextState()
        {
            if (Charecter.WayPoints.Count == 0)
            {
                if (Charecter.CellPosition == Charecter.Bar.ServiceCell)
                {
                    Charecter.Bar.CustomerArrived(Charecter);
                }

                Charecter.CurrentState = Charecter.IdleState;
            }
            else
            {
                // self transition
                Charecter.CurrentState = Charecter.MoveState;
            }
        }
    }
    #endregion
}


