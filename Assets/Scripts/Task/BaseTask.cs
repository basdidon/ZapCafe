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

    IWorkStation GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
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
            Started += delegate {
                WorkStation.Worker = Worker;
                TaskState = TaskStates.Pending;
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
            TaskManager.Instance.Tasks.Remove(this);
        };
    }

    public abstract float Duration { get; }

    public abstract IWorkStation GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
    public enum TaskStates { Created, Pending, Fulfilled }
    public TaskStates TaskState { get; private set; }
}