using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

[System.Serializable]
public abstract class Task
{
    [OdinSerialize] public Customer Customer { get; private set; }
    [OdinSerialize] public Worker Worker { get; set; }
    [OdinSerialize] TaskObject taskObject;
    public TaskObject TaskObject
    {
        get => taskObject;
        set
        {
            taskObject = value;
            started += delegate { TaskObject.Worker = Worker; };
            performed += delegate { TaskObject.Worker = null; };
        }
    }
    public abstract float Duration { get; }

    public Task(Customer customer) {
        Customer = customer;
        performed += delegate {
            Debug.Log("per");
            TaskManager.Instance.Tasks.Remove(this);
        };
    }

    public abstract bool TryGetTaskObject(Charecter charecter,out TaskObject taskObject);

    //public abstract Task Execute();
    public Action started;
    public Action performed;
}


/* 
public bool TryAssignTask(Worker worker)
{
    if (TryGetTaskObject(worker, out TaskObject taskObject))
    {
        // move worker to taskObject
        if (PathFinder.TryFindWaypoint(Worker, Worker.CellPosition, taskObject.WorkingCell, Worker.dirs, out List<Vector3Int> waypoints))
        {
            Worker = worker;
            TaskObject = taskObject;
            worker.CurrentState = new WorkerMove(worker, waypoints, new ExecutingTask(worker, taskObject));
            return true;
        }
        else
        {
            Debug.LogError("<color=red> Can't Move To TaskObject</color>");
        }
    }
    return false;
}*/