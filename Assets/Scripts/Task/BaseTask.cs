using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CharacterState.WorkerState;

public interface ITask 
{
    public Worker Worker { get; }
    public IWorkStation WorkStation { get; }

    float CreateAt { get; }
    int Depth { get; }

    float Duration { get; }

    bool TryGetWorkStation(Worker worker,out IWorkStation workStation);

    public Action Pending { get; set; }
    public Action Started { get; set; }
    public Action Cancled { get; set; }
    public Action Performed { get; set; }

    TaskStates TaskState { get; }

    void SelectWorker(IEnumerable<Worker> workers);
    void AssignWorker(Worker worker);
    void StartTask(Worker worker,IWorkStation workStation);
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
                    AssignWorker(task.Worker);
                }

                TaskManager.Instance.AddTask(this);
            }
        };
    }
}

public abstract class BaseTask : ITask
{
    [field: SerializeField] public Worker Worker { get; private set; }
    [field: SerializeField] public IWorkStation WorkStation { get; protected set; }

    public float CreateAt { get; }
    public int Depth { get; }

    public BaseTask(int parentDepth = 0)
    {
        CreateAt = Time.time;
        Depth = parentDepth;

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

    public abstract float Duration { get; }

    public Action Assigned { get; set; }
    public Action Pending { get; set; }
    public Action Started { get; set; }
    public Action Cancled { get; set; }
    public Action Performed { get; set; }

    [field: SerializeField] public TaskStates TaskState { get; private set; }

    public abstract bool TryGetWorkStation(Worker worker, out IWorkStation workStation);
    public abstract bool TryCheckCondition(Worker worker,IWorkStation workStation);

    public void AssignWorker(Worker worker)
    {
        Worker = worker;
        Assigned?.Invoke();
    }

    public void StartTask(Worker worker, IWorkStation workStation)
    {
        if (worker == null || workStation == null)
        {
            Debug.LogWarning("SetTask with null parameter");
            return;
        }

        Debug.Log($"SetTask to : {worker.name} + {workStation.WorkStationData.name}");

        if (worker.TryGetWaypoint(workStation.WorkingCell, out List<Vector3Int> waypoints))
        {
            Worker = worker;
            TaskManager.Instance.AvailableWorker.Remove(Worker);
            WorkStation = workStation;
            Worker.CurrentState = new MoveState(worker, waypoints, new ExecutingTask(worker,this));
            Pending?.Invoke();
        }
    }

    public void SelectWorker(IEnumerable<Worker> workers)
    {
        var result = workers.Select(
                worker =>
                {
                    var success = TryGetWorkStation(worker, out IWorkStation workStation) && TryCheckCondition(worker, workStation);
                    float distance = success ? workStation.RangeFrom(worker) : 0f;

                    return new { worker, workStation, distance, success };
                })
            .SkipWhile(item => !item.success)
            .OrderBy(item => item.distance)
            .FirstOrDefault();

        if (result != null)
            StartTask(result.worker, result.workStation);
    }
}

public enum TaskStates { 
    Created,        // default state when task was created.
    Assigned,       // when assigned worker but in a queue.
    Pending,        // already set workstation and worker.
    Started,        // when worker started execute task.
    Performed      // when this task finish.
}