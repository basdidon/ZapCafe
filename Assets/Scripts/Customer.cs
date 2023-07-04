using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class Customer : BoardObject,PathFinder.IMoveable
{
    [field:SerializeField] public Tilemap PathTilemap { get; set; }
    [SerializeField] List<Vector3Int> path;
    public List<Vector3Int> Path
    {
        get => path;
        set
        {
            path = value;
            OnNewPath?.Invoke();
        }
    }
    // Events
    public System.Action OnNewPath;
    public void OnNewPathHandle()
    {
        //CurrentState =
    }
    /*
    // State
    enum CustomerStates { Idle, Move, Ordered }

    [SerializeField] CustomerStates currentState;
    CustomerStates CurrentState
    {
        get => currentState;
        set
        {
            switch (value)
            {
                case CustomerStates.Move:
                    ;
                    

            }
            if (value != null)
            {
                CurrentState?.ExitState();
                currentState = value;
                CurrentState.EnterState();
            }
        }
    }
    */
    // Monobehaviour
    private void Start()
    {
        Debug.Log("Test()");
       // CurrentState = new CustomerIdleState();
    }

    public bool CanMoveTo(Vector3Int cellPos) => PathTilemap.HasTile(cellPos);

    [Button]
    public void GetPath(Vector3Int startCell,Vector3Int targetCell)
    {
        List<Vector3Int> dirs = new() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
        if (PathFinder.TryFindPath(this,startCell,targetCell, dirs, out List<Vector3Int> path))
        {
            Path = path;
        }
    }

    public void GetOrder()
    {
        
    }

    #region State
    public class CustomerIdleState : IState
    {
        public void EnterState(){}
        public void UpdateState(){}
        public void ExitState(){}
    }

    public class CustomerMoveState : IState
    {
        public void EnterState(){}
        public void UpdateState(){}
        public void ExitState(){}

    }
    #endregion
}
