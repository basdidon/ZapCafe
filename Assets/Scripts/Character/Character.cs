using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CharacterState;
using BasDidon.PathFinder;
using BasDidon.Direction;

public abstract class Character : BoardObject,IMoveable,IAnimationManipulated
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
        CurrentState ??= IdleState;
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

        AnimationManipulator.Play(animationHash);
    }
}