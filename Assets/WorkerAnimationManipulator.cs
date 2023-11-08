using UnityEngine;
using WorkerState;

public class WorkerAnimationManipulator : CharacterAnimationManipulator
{
    protected override void OnPlayAnimation()
    {
        Debug.Log($" OnPlayAnimation()");
        if (Animator == null)
            return;

        if (Charecter.CurrentState is MoveState<Worker>)
        {
            if (IsFront)
            {
                Animator.Play(AnimationHash.MoveFront);
            }
            else
            {
                Animator.Play(AnimationHash.MoveBack);
            }
        }
        else
        {
            if (IsFront)
            {
                Animator.Play(AnimationHash.IdleFront);
            }
            else
            {
                Animator.Play(AnimationHash.IdleBack);
            }
        }

    }
}