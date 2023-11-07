using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.PathFinder;
using BasDidon.Direction;

public static class AnimationHash
{
    public static int IdleFront => Animator.StringToHash("idle-front");
    public static int IdleBack => Animator.StringToHash("idle-back");
    public static int MoveFront => Animator.StringToHash("walk-front");
    public static int MoveBack => Animator.StringToHash("walk-back");
    public static int TalkFront => Animator.StringToHash("talk-front");
}

public abstract class Charecter : BoardObject,IMoveable
{
    public override Directions FacingDirection
    {
        get => base.FacingDirection;
        set
        {
            base.FacingDirection = value;

            if (Animator != null)
            {
                SpriteRenderer.flipX = (FacingDirection & (Directions.Down |Directions.Right)) == 0;
            }
        }
    }

    public void SetDirection(Vector3Int dir)
    {
        if (dir == Vector3Int.zero)
            return;
        FacingDirection = Direction.Vector3IntToDirection(dir);
    }

    public abstract bool CanMoveTo(Vector3Int cellPos);

    // SpriteRender
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    // Hold Item
    [SerializeField] SpriteRenderer ItemSpriteRenderer;
    // Animator
    [field :SerializeField] public Animator Animator { get; private set; }

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
            CurrentState.EnterState();
        }
    }

    public Vector3Int CellPos => CellPosition;

    protected virtual void Awake()
    {
        if(TryGetComponent(out Animator animator)){
            Animator = animator;
        }
        
        if(TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            SpriteRenderer = spriteRenderer;
        }
    }

    // Monobehaviour
    protected virtual void Start()
    {
        CurrentState = IdleState;
        HoldingItem = null;
    }
}

public class IdleState<T> : IState where T : Charecter
{
    protected T Charecter { get; }

    public IdleState(T charecter)
    {
        Charecter = charecter;
    }

    public virtual void EnterState(){
        Charecter.Animator.Play(AnimationHash.IdleFront);
    }
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
        Charecter.SetDirection(WayPoints[0] - BoardManager.Instance.GetCellPos(Charecter.transform.position));

        if (Charecter.Animator != null)
        {
            Charecter.Animator.StopPlayback();
            if ((Charecter.FacingDirection & (Directions.Down | Directions.Left)) != 0)
            {
                Charecter.Animator.Play(AnimationHash.MoveFront);
            }
            else
            {
                Debug.Log("back");
                Charecter.Animator.Play(AnimationHash.MoveBack);
            }
        }

        Charecter.StartCoroutine(MoveRoutine());
    }

    public virtual void ExitState(){}

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