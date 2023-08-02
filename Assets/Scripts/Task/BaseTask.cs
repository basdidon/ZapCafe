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

    float Duration { get; }

    bool TryGetWorkStation(Worker worker,out IWorkStation workStation);

    public Action Started { get; set; }
    public Action Performed { get; set; }

    TaskStates TaskState { get; }
    void AssignWorker(Worker worker);
    void SetTask(Worker[] workers);

    int GetSubTasks(List<ITask> tasks);
}

public abstract class BaseTask : ITask
{
    [field: SerializeField] public Worker Worker { get; private set; }
    [field: SerializeField] public IWorkStation WorkStation { get; private set; }
    List<Vector3Int> waypoints;
    public List<Vector3Int> Waypoints { get => waypoints; }

    public BaseTask()
    {
        Started += delegate {
            WorkStation.Worker = Worker;
            TaskState = TaskStates.Started;
        };
        Performed += delegate {
            WorkStation.Worker = null;
            TaskState = TaskStates.Performed;
            if (TaskManager.Instance.Tasks.Remove(this))
            {
                // Debug.Log($"{this.GetType()} was removed");
            }
        };
    }

    public abstract float Duration { get; }
    public abstract bool TryGetWorkStation(Worker worker, out IWorkStation workStation);

    public Action Started { get; set; }
    public Action Performed { get; set; }

    [field:SerializeField] public TaskStates TaskState { get; private set; }

    public void AssignWorker(Worker worker)
    {
        Worker = worker;
        TaskState = TaskStates.Assigned;
    }

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

        if(TryCheckCondition(ref pairs))
        {
            var result = pairs
                .OrderBy(pair => pair.Distance)
                .First();

            if (result == null)
                return;

            SetTask(result.Worker, result.WorkStation);
        }


    }

    // inverse decorator pattern
    public bool IsAllPrepareTasksDone { get; private set; }
    [SerializeReference] ITask[] prepareTasks;
    public ITask[] PrepareTasks 
    {
        get => prepareTasks;
        set
        {
            prepareTasks = value;
        }
    }

    public int GetSubTasks(List<ITask> tasks)
    {
        if (new TaskStates[] { TaskStates.Pending,TaskStates.Started,TaskStates.Performed }.Contains(TaskState))// != TaskStates.Created || TaskState != TaskStates.Assigned)
            return 0;

        if (PrepareTasks == null || PrepareTasks.Length == 0)
        {
            tasks.Add(this);
            return 1;
        }

        int n = 0;

        IsAllPrepareTasksDone = true;
        foreach (var _task in PrepareTasks)
        {
            n += _task.GetSubTasks(tasks);

            if (_task.TaskState != TaskStates.Performed)
                IsAllPrepareTasksDone = false;
        }

        if (n == 0 && IsAllPrepareTasksDone)
        {
            tasks.Add(this);
            n++;
        }

        return n;
        
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