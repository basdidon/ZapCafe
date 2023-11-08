using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.Direction;
using System;
using WorkerState;

public static class AnimationHash
{
    public static int IdleFront => Animator.StringToHash("idle-front");
    public static int IdleBack => Animator.StringToHash("idle-back");
    public static int MoveFront => Animator.StringToHash("walk-front");
    public static int MoveBack => Animator.StringToHash("walk-back");
    public static int TalkFront => Animator.StringToHash("talk-front");
}

public interface IAnimationManipulated
{
    public event Action<Direction> OnDirectionChangedEvent;
    public event Action<IState> OnStateChangedEvent;
    IState CurrentState { get; }
}

[RequireComponent(typeof(Charecter))]
public abstract class CharacterAnimationManipulator : MonoBehaviour
{
    protected IAnimationManipulated Charecter;
    
    // SpriteRender
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    // Animator
    [field: SerializeField] public Animator Animator { get; private set; }

    bool isFront = true;
    protected bool IsFront
    {
        get => isFront;
        set
        {
            isFront = value;
            OnPlayAnimation();
        }
    }

    private void Awake()
    {
        Charecter = GetComponent<Charecter>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Charecter.OnDirectionChangedEvent += OnDirectionChangedHandle;
        Charecter.OnStateChangedEvent += OnStateChangedHandle;
    }

    private void OnStateChangedHandle(IState newState)
    {
        Debug.Log($"StateChanged: {newState}");
        OnPlayAnimation();
    }

    private void OnDirectionChangedHandle(Direction newDirection)
    {
        Debug.Log($"DirectionChanged: {newDirection}");
        if (SpriteRenderer == null)
            return;

        SpriteRenderer.flipX = GridDirection.IsDirectionInGroup(newDirection, DirectionGroup.Right | DirectionGroup.Down);

        IsFront = GridDirection.IsDirectionInGroup(newDirection, DirectionGroup.Left | DirectionGroup.Down);
    }

    protected abstract void OnPlayAnimation();

    public void Play(int animationHash)
    {
        if (Animator == null)
            return;

        Debug.Log(" e");
        Animator.Play(animationHash);
    }
}