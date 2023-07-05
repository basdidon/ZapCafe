using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class Customer : BoardObject,PathFinder.IMoveable
{
    [field: SerializeField] public Tilemap PathTilemap { get; set; }
    [field: SerializeField] public List<Vector3Int> Path { get; set; }

    [SerializeField] List<Vector3Int> waypoints;
    public List<Vector3Int> WayPoints {
        get => waypoints; 
        set
        {
            waypoints = value;
            OnNewWaypoints?.Invoke();
        }
    }

    // Events
    public System.Action OnNewWaypoints;
    public void OnNewWaypointsHandle()
    {
        CurrentState = moveState;
    }
    
    // State
    enum CustomerStates { Idle, Move, Ordered }

    public CustomerIdleState idleState;
    public CustomerMoveState moveState;

    [SerializeField] IState currentState;
    IState CurrentState
    {
        get => currentState;
        set
        {
            if (value != null)
            {
                CurrentState?.ExitState();
                currentState = value;
                CurrentState.EnterState();
            }
        }
    }

    // Monobehaviour
    private void Awake()
    {
        idleState = new CustomerIdleState();
        moveState = new CustomerMoveState(this);

        OnNewWaypoints += OnNewWaypointsHandle;
    }

    private void Start()
    {
        Debug.Log("Test()");
        CurrentState = idleState;
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
        if(PathFinder.TryFindWaypoint(this,startCell,targetCell,dirs,out List<Vector3Int> wayPoints))
        {
            WayPoints = wayPoints;
            CurrentState = moveState;
        }
    }

    [Button]
    public void DebugGetPath()
    {
        GetPath(new Vector3Int(-8,-15,0),new Vector3Int(-6,-6,0));
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
        Customer Customer { get; }
        Vector3 startPos;
        Vector3 targetPos;
        float distance;
        readonly float speed = 1f;
        float duration;
        float timeElapsed;
        
        public CustomerMoveState(Customer customer)
        {
            Customer = customer;
        }

        public void EnterState(){
            Debug.Log("MoveState Started");
            Customer.StartCoroutine(MoveRoutine());
        }
        public void UpdateState(){}
        public void ExitState(){}
        
        IEnumerator MoveRoutine()
        {
            startPos = Customer.transform.position;
            targetPos = BoardManager.Instance.GetCellCenterWorld(Customer.WayPoints[0]);
            Customer.WayPoints.RemoveAt(0);

            // reset value
            distance = Vector3.Distance(startPos, targetPos);
            duration = distance / speed;
            timeElapsed = 0;

            while(timeElapsed < duration)
            {
                Customer.transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Customer.transform.position = targetPos;

            if(Customer.WayPoints.Count == 0)
            {
                Customer.CurrentState = Customer.idleState;
            }
            else
            {
                // self transition
                Customer.CurrentState = Customer.moveState;
            }
        }

    }
    #endregion
}
