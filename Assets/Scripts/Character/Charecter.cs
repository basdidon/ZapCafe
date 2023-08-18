using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirs { Up, Down, Left, Right }

public abstract class Charecter : BoardObject,PathFinder.IMoveable
{
    public readonly List<Vector3Int> dirs = new() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    [SerializeField] FacingDirs facingDir;
    public FacingDirs FacingDir
    {
        get => facingDir;
        set
        {
            facingDir = value;
            
            if (Animator != null)
            {
                Animator.SetBool("IsFront", FacingDir == FacingDirs.Down ||FacingDir == FacingDirs.Left);
                if (FacingDir == FacingDirs.Right || FacingDir == FacingDirs.Down)
                {
                    SpriteRenderer.flipX = true;
                    //transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    SpriteRenderer.flipX = false;
                    //transform.localScale = Vector3.one;
                }
            }
        }
    }

    public abstract bool CanMoveTo(Vector3Int cellPos);

    // SpriteRender
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    // Hold Item
    [SerializeField] SpriteRenderer ItemSpriteRenderer;
    // Animator
    [field :SerializeField] public Animator Animator { get; private set; }

    [SerializeField] Item holdingItem = null;
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
        var dir = WayPoints[0] - BoardManager.Instance.GetCellPos(Charecter.transform.position);
        Charecter.FacingDir = dir.y > 0 ? FacingDirs.Up : dir.x > 0 ? FacingDirs.Right : dir.x < 0 ? FacingDirs.Left : FacingDirs.Down;

        if(Charecter.Animator != null)
        {
            Charecter.Animator.SetBool("IsWalking", true);
            //Charecter.Animator.SetBool("IsFront", Charecter.FacingDir == FacingDirs.Down || Charecter.FacingDir == FacingDirs.Left);
        }

        Charecter.StartCoroutine(MoveRoutine());
    }

    public virtual void ExitState()
    {
        if (Charecter.Animator != null)
        {
            Charecter.Animator.SetBool("IsWalking", false);
        }
    }

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