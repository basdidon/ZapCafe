using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    TaskManager TaskManager { get => TaskManager.Instance; }
    public ITask Task { get; set; }

    // State
    IState currentState;
    public IState CurrentState {
        get => currentState;
        set
        {
            if (value != null)
            {
                CurrentState?.ExitState();
                currentState = value;
                CurrentState.EnterState();
            }
        }
    }

    private void Start()
    {
        TaskManager.AddAvaliableWorker(this);
    }

    public void SetTask(ITask task)
    {
        if(task != null)
        {
            Task = task;
        }
    }
}

public class WorkerIdle : IState
{
    public void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }
}

public class WorkerMove : IState
{
    Worker Worker { get; set; }
    Vector3 Target { get; set; }

    public WorkerMove(Worker worker, Vector3 target)
    {
        Worker = worker;
        Target = target;
    }

    public void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }
}

