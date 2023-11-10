using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using System.Linq;

public class TaskManager : SerializedMonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [OdinSerialize] public List<Worker> AvailableWorker { get; private set; }
    [OdinSerialize] List<ITask> Tasks { get; set; }

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
        Debug.Log($"Task '{newTask}' added.");
        Tasks.Add(newTask);
        TrySetTask();
    }

    public void RemoveTask(ITask task)
    {
        Tasks.Remove(task);
    }

    public bool IsWorkstationAvailable(IWorkStation workStation)
    {
        return !Tasks.Any(task => (task.TaskState == TaskStates.Started || task.TaskState == TaskStates.Pending) && task.WorkStation == workStation);
    } 

    public void AddAvaliableWorker(Worker worker)
    {
        if (worker == null)
            return;

        ITask assignedTask = Tasks.Find(task => task.TaskState == TaskStates.Assigned && task.Worker == worker);

        if (assignedTask != null && assignedTask.TryGetWorkStation(worker, out IWorkStation workStation))
        {
            assignedTask.StartTask(worker,workStation);
        }
        else
        {
            AvailableWorker.Add(worker);
            TrySetTask();
        }

        Debug.Log($" AddAvaliableWorker -> {worker.HoldingItem == null}");
    }

    public void TrySetTask()
    {
        if (AvailableWorker.Count <= 0)
            return;
        if (Tasks.Count <= 0)
            return;

        var orderedTasks = Tasks
            .Where(task=>task.TaskState == TaskStates.Created)
            .OrderBy(task => task.CreateAt)
            .ThenByDescending(task => task.Depth);

        foreach(var _task in orderedTasks)
        {
            if (AvailableWorker.Count < 0)
                break;

            _task.SelectWorker(AvailableWorker);
        }
    }
}
