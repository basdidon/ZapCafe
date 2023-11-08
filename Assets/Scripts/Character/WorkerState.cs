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
            base.EnterState();
            TaskManager.Instance.AddAvaliableWorker(Charecter);
        }
    }

    public class MoveState : MoveState<Worker>
    {
        public IState NextState { get; set; }
        public MoveState(Worker worker, List<Vector3Int> waypoints, IState nextState = null) : base(worker, waypoints) 
            => NextState = nextState;

        public override void SetNextState()
        {
            Charecter.CurrentState = WayPoints.Count == 0 ? NextState : new MoveState(Charecter, WayPoints, NextState);
        }
    }

    public class ExecutingTask : ISelfExitState
    {
        Worker Worker { get; }
        ITask CurrentTask { get; }
        float timeElapsed;
        float duration = 1f;

        public ExecutingTask(Worker worker,ITask currentTask)
        {
            Worker = worker;
            CurrentTask = currentTask;
        }

        public void EnterState()
        {
            Worker.TaskProgress.SetActive(true);
            Worker.ProgressImg.fillAmount = 0f;
            if (Worker == null) Debug.Log("worker == null");
            if (CurrentTask == null) Debug.Log("Task == null");
            
            duration = CurrentTask.Duration;
            timeElapsed = 0f;

            Worker.SetDirection(CurrentTask.WorkStation.CellPosition - BoardManager.Instance.GetCellPos(Worker.transform.position));
            Worker.StartCoroutine(StartTask());
        }

        IEnumerator StartTask()
        {
            CurrentTask.Started?.Invoke();
            while (timeElapsed < duration)
            {
                Worker.ProgressImg.fillAmount = timeElapsed / duration;
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Debug.Log($"{Worker.name} performed {CurrentTask}");
            CurrentTask.Performed?.Invoke();

            SetNextState();
        }

        public void ExitState()
        {
            Worker.TaskProgress.SetActive(false);
        }

        public void SetNextState()
        {
            Worker.CurrentState = Worker.IdleState;
        }
    }
}
