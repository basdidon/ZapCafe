using UnityEngine;
using CharacterState.WorkerState;

public class WorkerAnimationManipulator : CharacterAnimationManipulator
{
    protected override void OnPlayAnimation()
    {
        if (Animator == null)
            return;

        if (Charecter.CurrentState is MoveState)
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