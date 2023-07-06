using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public abstract class Charecter : BoardObject,PathFinder.IMoveable
{
    [SerializeField] List<Vector3Int> waypoints;
    public List<Vector3Int> WayPoints
    {
        get => waypoints;
        set
        {
            waypoints = value;
            OnNewWaypoints?.Invoke();
        }
    }

    public abstract bool CanMoveTo(Vector3Int cellPos);

    // Events
    public System.Action OnNewWaypoints;
    public virtual void OnNewWaypointsHandle()
    {
        CurrentState = MoveState;
    }

    // State
    public IState IdleState { get; set; }
    public IState MoveState { get; set; }

    [SerializeField] IState currentState;
    public IState CurrentState
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
    protected virtual void Awake()
    {
        OnNewWaypoints += OnNewWaypointsHandle;
    }

    protected virtual void Start()
    {
        CurrentState = IdleState;
    }
}

public class IdleState<T> : IState
{
    protected T Charecter { get; }

    public IdleState(T charecter)
    {
        Charecter = charecter;
    }

    public virtual void EnterState(){}
    public virtual void ExitState(){}
}

public abstract class MoveState<T> : ISelfExitState where T : Charecter
{
    protected T Charecter { get; }
    Vector3 startPos;
    Vector3 targetPos;
    float distance;
    readonly float speed = 1f;
    float duration;
    float timeElapsed;

    public MoveState(T charecter)
    {
        Charecter = charecter;
    }

    public virtual void EnterState()
    {
        Debug.Log("MoveState Started");
        Charecter.StartCoroutine(MoveRoutine());
    }
    public virtual void ExitState() { }

    IEnumerator MoveRoutine()
    {
        startPos = Charecter.transform.position;
        targetPos = BoardManager.Instance.GetCellCenterWorld(Charecter.WayPoints[0]);
        Charecter.WayPoints.RemoveAt(0);

        // reset value
        distance = Vector3.Distance(startPos, targetPos);
        duration = distance / speed;
        timeElapsed = 0;

        while (timeElapsed < duration)
        {
            Charecter.transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Charecter.transform.position = targetPos;

        ExitConditionCheck();
    }

    public abstract void ExitConditionCheck();
}