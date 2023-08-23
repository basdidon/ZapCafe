using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using System.Linq;

public class TaskManager : SerializedMonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [OdinSerialize] public List<ITask> Tasks { get; private set; }
    [OdinSerialize] public List<Worker> AvailableWorker { get; private set; }
    [OdinSerialize] public List<ITask> AvailableTasks { get; set; }

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
        AvailableTasks = new();

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

        var orderedTasks = AvailableTasks
            .Where(task=>task.TaskState == TaskStates.Created)
            .OrderBy(task => task.CreateAt)
            .ThenByDescending(task => task.Depth);

        foreach(var _task in orderedTasks)
        {
            if (AvailableWorker.Count < 0)
                break;

            Debug.Log("asadafa");
            _task.SetTask(AvailableWorker.ToArray());
            
        }
    }
}
