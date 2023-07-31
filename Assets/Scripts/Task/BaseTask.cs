using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading.Tasks;

public interface ITask
{
    public Worker Worker { get; set; }
    public IWorkStation WorkStation { get; set; }

    float Duration { get; }

    bool TryGetWorkStation(Worker worker,out IWorkStation workStation);

    public Action Started { get; set; }
    public Action Performed { get; set; }
    public TaskStates TaskState { get; }

    ITask GetTask();
    int GetSubTasks(List<ITask> tasks);
}

public abstract class BaseTask : ITask
{
    [field:SerializeField] public Worker Worker { get; set; }
    [OdinSerialize] IWorkStation workStation;
    public IWorkStation WorkStation
    {
        get => workStation;
        set
        {
            workStation = value;

            TaskState = TaskStates.Pending;

            Started += delegate {
                WorkStation.Worker = Worker;
                TaskState = TaskStates.Started;
            };
            Performed += delegate {
                WorkStation.Worker = null;
                TaskState = TaskStates.Fulfilled;
            };
        }
    }
    
    public BaseTask()
    {
        Performed += delegate {
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

    public TaskStates TaskState { get; private set; }

    // inverse decorator pattern
    public bool IsAllPrepareTasksDone { get; private set; }
    [SerializeReference] ITask[] prepareTasks;
    public ITask[] PrepareTasks
    {
        get => prepareTasks;
        set
        {
            prepareTasks = value;

            if (PrepareTasks != null && PrepareTasks.Length > 0)
            {
                foreach (var _task in PrepareTasks)
                {
                    _task.Performed += delegate
                    {
                        IsAllPrepareTasksDone = true;
                        foreach (var _task in PrepareTasks)
                        {
                            if (_task.TaskState != TaskStates.Fulfilled)
                            {
                                IsAllPrepareTasksDone = false;
                                break;
                            }
                        }
                    };
                }
            }
        }
    }
    public ITask GetTask()
    {
        if (PrepareTasks != null && PrepareTasks.Length > 0)
        {
            foreach(var _task in PrepareTasks)
            {
                if (_task.TaskState == TaskStates.Created)
                    return _task;
            }
        }

        return this;
    }

    public int GetSubTasks(List<ITask> tasks)
    {
        if (TaskState != TaskStates.Created)
            return 0;

        if (PrepareTasks == null || PrepareTasks.Length == 0)
        {
            tasks.Add(this);
            return 1;
        }
        else
        {
            int n = 0;

            foreach (var _task in PrepareTasks)
            {
                n += _task.GetSubTasks(tasks);
            }

            if (n == 0 && IsAllPrepareTasksDone)
            {
                tasks.Add(this);
                n++;
            }

            return n;
        }
    }
}

public enum TaskStates { Created, Pending, Started, Fulfilled }