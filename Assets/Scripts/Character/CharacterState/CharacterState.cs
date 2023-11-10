using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterState
{
    public interface IState
    {
        public void EnterState();
        public void UpdateState();
        public void ExitState();
    }

    public interface ISelfExitState : IState
    {
        public void SetNextState();
    }

    public class IdleState<T> : IState where T : Character
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


    public abstract class MoveState<T> : ISelfExitState where T : Character
    {
        protected T Charecter { get; }
        [field: SerializeField] protected List<Vector3Int> WayPoints { get; set; }
        Vector3 startPos;
        Vector3 targetPos;
        float distance;
        readonly float speed = 1f;
        float duration;
        float timeElapsed;

        public MoveState(T charecter, List<Vector3Int> waypoints)
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
            if (timeElapsed < duration)
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

        public virtual void ExitState() { }

        public abstract void SetNextState();
    }
}