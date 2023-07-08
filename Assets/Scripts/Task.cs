using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[System.Serializable]
public abstract class Task
{
    public Customer Customer { get; }
    //public Worker Worker { get; private set; }
    //public TaskObject TaskObject { get; private set; }

    public abstract float Duration { get; }
    
    public Task(Customer customer)
    {
        Customer = customer;
    }

    public abstract bool TryGetTaskObject(Charecter charecter,out TaskObject taskObject);
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
    
    public abstract Task Execute();
}