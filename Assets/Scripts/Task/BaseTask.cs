using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using WorkerState;

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

    TaskStates TaskState { get;}
    void SetTask(IEnumerable<Worker> workers);
    void SetTask(Worker worker,IWorkStation workStation);
    void AssignWorker(Worker worker);
}

public interface IDependentTask : ITask
{
    IEnumerable<ITask> DependencyTasks { get; set; }

    public void SetDependencyTasks(ITask task)
    {
        DependencyTasks = new ITask[] { task };
        SetupDependencyTask(task);
    }
    public void SetDependencyTasks(ITask[] tasks)
    {
        if(tasks == null || tasks.Length <= 0)
        {
            Debug.LogWarning("tasks can't be null or empty");
            return;
        }

        DependencyTasks = tasks;
        foreach (var task in DependencyTasks)
        {
            SetupDependencyTask(task);
        }
    }

    void SetupDependencyTask(ITask task)
    {
        task.Performed += delegate
        {
            if (DependencyTasks.All(_task => _task.TaskState == TaskStates.Performed))
            {
                if (TryGetWorkStation(task.Worker, out IWorkStation workStation))
                {
                    this.AssignWorker(task.Worker);
                }
                TaskManager.Instance.AddTask(this);
            }
        };
    }
}

public abstract class BaseTask : ITask
{
    [field: SerializeField] public Worker Worker { get; private set; }
    [field: SerializeField] public IWorkStation WorkStation { get; set; }
    List<Vector3Int> waypoints;
    public List<Vector3Int> Waypoints { get => waypoints; }

    public float CreateAt { get; }
    public int Depth { get; }

    public BaseTask()
    {
        CreateAt = Time.time;
        Depth = 0;

        Assigned += delegate { TaskState = TaskStates.Assigned; };
        Pending += delegate { TaskState = TaskStates.Pending; };
        Started += delegate { TaskState = TaskStates.Started; };

        Cancled += delegate
        {
            Worker.CurrentState = Worker.IdleState;
            Worker = null;
            WorkStation = null;
            TaskState = TaskStates.Created;
        };

        Performed += delegate {
            TaskState = TaskStates.Performed;
            TaskManager.Instance.RemoveTask(this);
        };
    }

    public BaseTask(int parentDepth):this()
    {
        Depth = parentDepth+1;
    }

    public abstract float Duration { get; }
    public abstract bool TryGetWorkStation(Worker worker, out IWorkStation workStation);

    public Action Assigned { get; set; }
    public Action Pending { get; set; }
    public Action Started { get; set; }
    public Action Cancled { get; set; }
    public Action Performed { get; set; }

    [field: SerializeField] public TaskStates TaskState { get; private set; }

    public abstract bool TryCheckCondition(Worker worker,IWorkStation workStation);

    public void AssignWorker(Worker worker)
    {
        Worker = worker;
        Assigned?.Invoke();
    }

    public void SetTask(Worker worker, IWorkStation workStation)
    {
        if (worker == null || workStation == null)
        {
            Debug.LogWarning("SetTask with null parameter");
            return;
        }

        Debug.Log($"SetTask to : {worker.name} + {workStation.WorkStationData.name}");

        if (worker.TryGetWaypoint(workStation.WorkingCell, out List<Vector3Int> _waypoints))
        {
            waypoints = _waypoints;
            Worker = worker;
            TaskManager.Instance.AvailableWorker.Remove(Worker);
            WorkStation = workStation;
            Worker.CurrentState = new MoveState(worker, Waypoints, new ExecutingTask(worker,this));
            Pending?.Invoke();
        }
    }

    public void SetTask(IEnumerable<Worker> workers)
    {
        WorkerWorkStationPair result = workers.Select(
                worker =>
                {
                    if (TryGetWorkStation(worker, out IWorkStation workStation) && TryCheckCondition(worker, workStation))
                    {
                        var distance = workStation.RangeFrom(worker);
                        return new WorkerWorkStationPair(worker, workStation, distance);
                    }
                    return null;
                })
            .SkipWhile(pair => pair == null || pair.WorkStation == null)
            .OrderBy(pair => pair.Distance)
            .FirstOrDefault();

        if (result == null)
            return;

        SetTask(result.Worker, result.WorkStation);
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
    public float Distance { get; }

    public WorkerWorkStationPair(Worker worker, IWorkStation workStation, float distance)
    {
        Worker = worker;
        WorkStation = workStation;
        Distance = distance;
    }
}