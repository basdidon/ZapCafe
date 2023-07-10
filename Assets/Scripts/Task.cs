using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

public interface ITask<out T> where T : WorkStation
{
    Customer Customer { get; }
    Worker Worker { get; set; }
    WorkStation WorkStation { get; set; }

    float Duration { get; }
    bool TryGetworkStation(Worker worker, out WorkStation workStation);

    public Action started { get; set; }
    public Action performed { get; set; }
}

[System.Serializable]
public abstract class Task<T> : ITask<T> where T : WorkStation
{
    [OdinSerialize] public Customer Customer { get; }
    [OdinSerialize] public Worker Worker { get; set; }
    [OdinSerialize] WorkStation workStation;
    public WorkStation WorkStation
    {
        get => workStation;
        set
        {
            workStation = value;
            started += delegate { WorkStation.Worker = Worker; };
            performed += delegate { WorkStation.Worker = null; };
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

    public abstract bool TryGetworkStation(Worker worker, out WorkStation workStation);

    public Action started { get; set; }
    public Action performed { get; set; }
}

/*
public abstract class WorkStation{}

public class WorkStationA : WorkStation{ }
public class WorkStationB : WorkStation{ }
*/

/*
public sealed class TaskA<T> : ITask<T> where T : WorkStationA
{
    public T WorkStation { get; set; }
}

public sealed class TaskB<T> : ITask<T> where T : WorkStationB
{
    public T WorkStation { get; set; }
}
*/


/* 
public bool TryAssignTask(Worker worker)
{
    if (TryGetworkStation(worker, out workStation workStation))
    {
        // move worker to workStation
        if (PathFinder.TryFindWaypoint(Worker, Worker.CellPosition, workStation.WorkingCell, Worker.dirs, out List<Vector3Int> waypoints))
        {
            Worker = worker;
            workStation = workStation;
            worker.CurrentState = new WorkerMove(worker, waypoints, new ExecutingTask(worker, workStation));
            return true;
        }
        else
        {
            Debug.LogError("<color=red> Can't Move To workStation</color>");
        }
    }
    return false;
}*/