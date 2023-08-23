using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public interface ITask
{
    public Worker Worker { get; }
    public IWorkStation WorkStation { get; }
    public List<Vector3Int> Waypoints { get; }

    float CreateAt { get; }
    int Depth { get; }

    float Duration { get; }

    bool TryGetWorkStation(Worker worker,out IWorkStation workStation);

    public Action Pending { get; set; }
    public Action Started { get; set; }
    public Action Cancled { get; set; }
    public Action Performed { get; set; }

    TaskStates TaskState { get; }
    void SetTask(Worker[] workers);
}

public abstract class BaseTask : ITask
{
    [field: SerializeField] public Worker Worker { get; private set; }
    [field: SerializeField] public IWorkStation WorkStation { get; set; }
    List<Vector3Int> waypoints;
    public List<Vector3Int> Waypoints { get => waypoints; }

    public float CreateAt { get; }
    public int Depth { get; }

    ITask[] DependencyTasks { get; set; }

    public BaseTask()
    {
        CreateAt = Time.time;
        Depth = 0;

        Started += delegate {
            WorkStation.Worker = Worker;
            TaskState = TaskStates.Started;
        };

        Cancled += delegate
        {
            Worker.CurrentTask = null;
            Worker.CurrentState = Worker.IdleState;
            Worker = null;
            WorkStation.Worker = null;
            WorkStation = null;
            TaskState = TaskStates.Created;

        };

        Performed += delegate {
            WorkStation.Worker = null;
            TaskState = TaskStates.Performed;
            TaskManager.Instance.Tasks.Remove(this);
            TaskManager.Instance.AvailableTasks.Remove(this);
        };
    }

    public BaseTask(int parentDepth):this()
    {
        Depth = parentDepth+1;
    }

    //protected abstract void SetDependencyTasks();
    protected void SetDependencyTasks(ITask[] tasks =null)
    {
        DependencyTasks = tasks;
        if (DependencyTasks == null || DependencyTasks.Length <= 0)
        {
            TaskManager.Instance.AvailableTasks.Add(this);
        }
        else
        {
            foreach (var task in DependencyTasks)
            {
                task.Performed += delegate
                {
                    if (DependencyTasks.All(_task => _task.TaskState == TaskStates.Performed))
                    {
                        if (TryGetWorkStation(task.Worker, out IWorkStation workStation))
                        {
                            SetTask(task.Worker, workStation);
                        }
                        else
                        {
                            TaskManager.Instance.AvailableTasks.Add(this);
                        }

                    }
                };
            }
        }
    }

    public abstract float Duration { get; }
    public abstract bool TryGetWorkStation(Worker worker, out IWorkStation workStation);

    public Action Pending { get; set; }
    public Action Started { get; set; }
    public Action Cancled { get; set; }
    public Action Performed { get; set; }

    [field: SerializeField] public TaskStates TaskState { get; private set; }

    public abstract IEnumerable<WorkerWorkStationPair> GetTaskCondition(IEnumerable<WorkerWorkStationPair> pairs);
    public abstract bool TryCheckCondition(ref IEnumerable<WorkerWorkStationPair> pairs);

    protected void SetTask(Worker worker, IWorkStation workStation)
    {
        if (worker == null || workStation == null)
            return;

        if (worker.TryGetWaypoint(workStation.WorkingCell, out List<Vector3Int> _waypoints))
        {
            waypoints = _waypoints;
            Worker = worker;
            TaskManager.Instance.AvailableWorker.Remove(Worker);
            WorkStation = workStation;
            Worker.CurrentTask = this;
            WorkStation.Worker = Worker;
            TaskState = TaskStates.Pending;
        }
    }

    public void SetTask(Worker[] workers)
    {
        var pairs = workers.Select(
            worker =>
            {
                if (TryGetWorkStation(worker, out IWorkStation workStation))
                {
                    var distance = workStation.SqrMagnitude(worker);
                    return new WorkerWorkStationPair(worker, workStation, distance);
                }
                return null;
            })
            .Where(pair => pair != null && pair.WorkStation != null);

        if (TryCheckCondition(ref pairs))
        {
            var result = pairs
                .OrderBy(pair => pair.Distance)
                .First();

            if (result == null)
                return;

            SetTask(result.Worker, result.WorkStation);
        }
    }
}

public enum TaskStates { 
    Created,        // default state when task was created.
    Assigned,       // when assigned worker but in a queue.
    Pending,        // already set workstation and worker.
    Started,        // when worker started execute task.
    Performed      // when this task finish.
}

public class WorkerWorkStationPair
{
    public Worker Worker { get; }
    public IWorkStation WorkStation { get; }
    public float? Distance { get; }

    public WorkerWorkStationPair(Worker worker, IWorkStation workStation, float? distance)
    {
        Worker = worker;
        WorkStation = workStation;
        Distance = distance;
    }
}
/*
public class DependentTasks
{
    public ITask SuccessorTask { get; set; }
    public ITask[] ProcedecessorTasks { get; set; }

    public DependentTasks(ITask successorTask,ITask[] )
    {
        ProcedecessorTask = procedecessorTask;
        DependencyTasks = dependencyTasks;
    }
}*/