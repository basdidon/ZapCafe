using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class TaskManager : SerializedMonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [OdinSerialize] public List<ITask> Tasks { get; private set; }
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

    public void AddTask(ITask newTask)
    {
        if (newTask == null)
            return;

        Tasks.Add(newTask);
        TrySetTask();
    }

    public void AddAvaliableWorker(Worker worker)
    {
        if (worker == null)
            return;

        AvailableWorker.Add(worker);
        TrySetTask();
    }

    public void TrySetTask()
    {
        if (AvailableWorker.Count <= 0)
            return;
        if (Tasks.Count <= 0)
            return;

        UpdateAvailableTasks();
        foreach(var _task in availableTasks)
        {
            if (AvailableWorker.Count < 0)
                break;

            _task.SetTask(AvailableWorker.ToArray());
            
        }
    }

    public List<ITask> availableTasks = new();
    public List<ITask> UpdateAvailableTasks()
    {
        availableTasks.Clear();
        Tasks.ForEach(task => task.GetSubTasks(availableTasks));
        return availableTasks;
    }
}
