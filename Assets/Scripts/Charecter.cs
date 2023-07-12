using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public abstract class Charecter : BoardObject,PathFinder.IMoveable
{
     public readonly List<Vector3Int> dirs = new() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
    public abstract bool CanMoveTo(Vector3Int cellPos);

    // Hold Item
    [SerializeField] SpriteRenderer ItemSpriteRenderer;

    Item holdingItem = null;
    public Item HoldingItem
    {
        get => holdingItem;
        set
        {
            holdingItem = value;
            if (HoldingItem == null)
            {
                ItemSpriteRenderer.sprite = null;
            }
            else
            {
                Debug.Log(HoldingItem);
                Debug.Log(HoldingItem.Sprite.ToString());
                ItemSpriteRenderer.sprite = HoldingItem.Sprite;
            }
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
            CurrentState.EnterState();
        }
    }

    // Monobehaviour
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
        Charecter.StartCoroutine(MoveRoutine());
    }

    public virtual void ExitState() { }

    IEnumerator MoveRoutine()
    {
        startPos = Charecter.transform.position;
        targetPos = BoardManager.Instance.GetCellCenterWorld(WayPoints[0]);
        WayPoints.RemoveAt(0);

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

        SetNextState();
    }

    public abstract void SetNextState();
}