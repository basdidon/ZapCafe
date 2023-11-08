using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BasDidon.PathFinder;
using BasDidon.Direction;

public abstract class Charecter : BoardObject,IMoveable,IAnimationManipulated
{
    public override Direction FacingDirection
    {
        get => base.FacingDirection;
        set
        {
            base.FacingDirection = value;
            OnDirectionChangedEvent?.Invoke(base.FacingDirection);
        }
    }

    public void SetDirection(Vector3Int dir)
    {
        if (dir == Vector3Int.zero)
            return;
        FacingDirection = GridDirection.Vector3IntToDirection(dir);
    }

    public abstract bool CanMoveTo(Vector3Int cellPos);

    // IAnimationManipulated
    public event Action<Direction> OnDirectionChangedEvent;
    public event Action<IState> OnStateChangedEvent;

    // Hold Item
    [SerializeField] SpriteRenderer ItemSpriteRenderer;

    public CharacterAnimationManipulator AnimationManipulator { get;private set; }

    Item holdingItem;  // don't add [SerializeField] to this, it make this always not null after first update.
    public Item HoldingItem
    {
        get => holdingItem;
        set
        {
            holdingItem = value;

            ItemSpriteRenderer.sprite = HoldingItem?.Sprite;
        }
    }

    // State
    public IState IdleState { get; set; }

    [SerializeReference] protected IState currentState;
    public IState CurrentState
    {
        get => currentState;
        set
        {
            CurrentState?.ExitState();
            currentState = value ?? IdleState;
            OnStateChangedEvent?.Invoke(CurrentState);  // invoke stateChangedEvent before it start
            CurrentState.EnterState();
        }
    }

    public Vector3Int CellPos => CellPosition;

    protected virtual void Awake()
    {
        if (TryGetComponent(out CharacterAnimationManipulator animationManipulator))
        {
            AnimationManipulator = animationManipulator;
        }
    }

    // Monobehaviour
    protected virtual void Start()
    {
        CurrentState = IdleState;
        HoldingItem = null;

        // snap to grid
        transform.position = BoardManager.Instance.GetCellCenterWorld(CellPos);
    }

    private void Update()
    {
        CurrentState?.UpdateState();
    }

    public void PlayAnimation(int animationHash)
    {
        if (AnimationManipulator == null)
            return;

        Debug.Log("s");
        AnimationManipulator.Play(animationHash);
    }
}

public class IdleState<T> : IState where T : Charecter
{
    protected T Charecter { get; }

    public IdleState(T charecter)
    {
        Charecter = charecter;
    }

    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}

public abstract class MoveState<T> : ISelfExitState where T : Charecter
{
    protected T Charecter { get; }
    [field: SerializeField] protected List<Vector3Int> WayPoints { get; set; }
    Vector3 startPos;
    Vector3 targetPos;
    float distance;
    readonly float speed = 1f;
    float duration;
    float timeElapsed;

    public MoveState(T charecter,List<Vector3Int> waypoints)
    {
        Charecter = charecter;
        WayPoints = waypoints;
    }

    public virtual void EnterState()
    {
        Charecter.SetDirection(WayPoints[0] - BoardManager.Instance.GetCellPos(Charecter.transform.position));

        // setup
        startPos = Charecter.transform.position;
        targetPos = BoardManager.Instance.GetCellCenterWorld(WayPoints[0]);
        WayPoints.RemoveAt(0);

        // reset value
        distance = Vector3.Distance(startPos, targetPos);
        duration = distance / speed;
        timeElapsed = 0;
    }

    public void UpdateState()
    {
        if(timeElapsed < duration)
        {
            Charecter.transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
        }
        else
        {
            Charecter.transform.position = targetPos;

            SetNextState();
        }
    }

    public virtual void ExitState(){}

    public abstract void SetNextState();
}