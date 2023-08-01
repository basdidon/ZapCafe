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

    public TaskStates TaskState { get; }
    public void AssignWorker(Worker worker);
    public void SetTask(Worker[] workers);

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
                Debug.Log($"{this.GetType()} was removed");
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

    public void SetTask(Worker[] workers)
    {
        if (workers == null || workers.Length <= 0)
            return;

        Worker targetWorker = null;
        IWorkStation targetWorkStation = null;

        if (TaskState == TaskStates.Assigned)
        {
            foreach(var _worker in workers)
            {
                if(Worker == _worker && TryGetWorkStation(_worker,out IWorkStation workStation))
                {
                    targetWorker = _worker;
                    targetWorkStation = workStation;
                }
            }
        }
        else
        {
            // find closest worker & workstation
            float minSqrMagnitude = Mathf.Infinity;
            foreach (var _worker in workers)
            {
                if (TryGetWorkStation(_worker, out IWorkStation workStation))
                {
                    float sqrMegnitude = workStation.SqrMagnitude(_worker);
                    if (sqrMegnitude < minSqrMagnitude)
                    {
                        targetWorker = _worker;
                        targetWorkStation = workStation;
                        minSqrMagnitude = sqrMegnitude;
                    }
                }
            }
        }

        if(targetWorker != null && targetWorkStation != null)
        {
            if(targetWorker.TryGetWaypoint(targetWorkStation.WorkingCell,out List<Vector3Int> _waypoints))
            {
                waypoints = _waypoints;
                Worker = targetWorker;
                TaskManager.Instance.AvailableWorker.Remove(Worker);
                WorkStation = targetWorkStation;
                Worker.CurrentTask = this;
                WorkStation.Worker = Worker;
                TaskState = TaskStates.Pending;
            }
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