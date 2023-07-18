using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
/*
public interface ITask<out T> where T : Item {
    public Customer Customer { get; }
    public Worker Worker { get; set; }
    public IWorkStation<Item> WorkStation { get; set; }

    float Duration { get; }

    IWorkStation<T> GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
}
*/
public interface ITask
{
    public Customer Customer { get; }
    public Worker Worker { get; set; }
    public IWorkStation WorkStation { get; set; }

    float Duration { get; }

    IWorkStation GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
}


public abstract class Task  : ITask
{
    public Customer Customer { get; }
    public Worker Worker { get; set; }
    [OdinSerialize] IWorkStation workStation;
    public IWorkStation WorkStation
    {
        get => workStation;
        set
        {
            workStation = value;
            Started += delegate { WorkStation.Worker = Worker; };
            Performed += delegate { WorkStation.Worker = null; };
        }
    }

    public Task(Customer customer)
    {
        Customer = customer;
        Performed += delegate {
            TaskManager.Instance.Tasks.Remove(this);
        };
    }

    public abstract float Duration { get; }

    public abstract IWorkStation GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
}