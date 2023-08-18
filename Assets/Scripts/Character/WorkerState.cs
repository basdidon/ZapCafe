using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkerState
{
    public class IdleState : IdleState<Worker>
    {
        public IdleState(Worker worker) : base(worker) { }

        public override void EnterState()
        {
            Debug.Log("Player Idle");
            Charecter.Animator.SetBool("IsWalking", false);
            Charecter.Animator.SetBool("IsTalking", false);
            TaskManager.Instance.AddAvaliableWorker(Charecter);
        }

        public override void ExitState() { }
    }

    public class MoveState : MoveState<Worker>
    {
        public IState NextState { get; set; }
        public MoveState(Worker worker, List<Vector3Int> waypoints, IState nextState = null) : base(worker, waypoints)
        {
            NextState = nextState;
        }

        public override void SetNextState()
        {
            if (WayPoints.Count == 0)
            {
                Charecter.CurrentState = NextState;
            }
            else
            {
                // self transition
                Charecter.CurrentState = new MoveState(Charecter, WayPoints, NextState);
            }
        }
    }

    public class ExecutingTask : ISelfExitState
    {
        Worker Worker { get; }
        float timeElapsed;
        float duration;

        public ExecutingTask(Worker worker)
        {
            Worker = worker;
        }

        public void EnterState()
        {
            Worker.TaskProgress.SetActive(true);
            Worker.ProgressImg.fillAmount = 0f;
            duration = Worker.CurrentTask.Duration;
            timeElapsed = 0f;

            var dir = Worker.CurrentTask.WorkStation.CellPosition - BoardManager.Instance.GetCellPos(Worker.transform.position);
            Worker.FacingDir = dir.y > 0 ? FacingDirs.Up : dir.x > 0 ? FacingDirs.Right : dir.x < 0 ? FacingDirs.Left : FacingDirs.Down;

            Worker.StartCoroutine(StartTask());
        }

        IEnumerator StartTask()
        {
            Worker.CurrentTask.Started?.Invoke();
            while (timeElapsed < duration)
            {
                Worker.ProgressImg.fillAmount = timeElapsed / duration;
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Debug.Log($"{Worker.name} performed {Worker.CurrentTask}");
            Worker.CurrentTask.Performed?.Invoke();

            SetNextState();
        }

        public void ExitState()
        {
            Worker.TaskProgress.SetActive(false);
            Worker.CurrentTask = null;
        }

        public void SetNextState()
        {
            Worker.CurrentState = Worker.IdleState;
        }
    }
}
