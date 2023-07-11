using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class TaskManager : SerializedMonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [OdinSerialize] public List<ITask<Item>> Tasks { get; private set; }
    [OdinSerialize] public List<Worker> AvailableWorker { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Tasks = new();
        AvailableWorker = new();
    }

    public void AddTask(ITask<Item> newTask)
    {
        if (newTask == null)
            return;

        var worker = AvailableWorker.Find(worker => worker.TrySetTask(newTask));

        if(worker != null)
        {
            AvailableWorker.Remove(worker);
        }

        Tasks.Add(newTask);        
    }

    public void AddAvaliableWorker(Worker worker)
    {
        if (worker == null)
            return;
        
        var task = Tasks.Find(task => task.Worker == null && worker.TrySetTask(task));

        if (task == null) 
        {
            AvailableWorker.Add(worker);
        }
    }

    public void WorkStationFree<T>() where T : Item
    {
        if (AvailableWorker.Count <= 0)
            return;

        var worker = AvailableWorker[0];
        var task = Tasks.Find(task => task.Worker == null && worker.TrySetTask(task));

        if (task == null)
        {
            AvailableWorker.Add(worker);
        }
        AvailableWorker.Remove(worker);

    }
}
