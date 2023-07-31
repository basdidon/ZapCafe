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
        /*
        var worker = AvailableWorker.Find(worker => worker.TrySetTask(newTask.GetTask()));

        if(worker != null)
        {
            AvailableWorker.Remove(worker);
        }
        */
        Tasks.Add(newTask);
        TryAssignTask();
    }

    public void AddAvaliableWorker(Worker worker)
    {
        if (worker == null)
            return;
        /*
        var task = Tasks.Find(task => CompareDebug(task.Worker, worker) && worker.TrySetTask(task));

        if(task == null)
            task = Tasks.Find(task => task.Worker == null && worker.TrySetTask(task));

        if (task == null)
        {
        }*/
        AvailableWorker.Add(worker);
        TryAssignTask();
    }

    bool CompareDebug(Worker a,Worker b)
    {
        var sa = a != null ? a.name : "null";
        var sb = b != null ? b.name : "null";

        bool result = a == b;
        Debug.Log($"{sa} == {sb} => {result}");
        return result;
    }

    public void WorkStationFree()
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

    public void TryAssignTask()
    {
        if (AvailableWorker.Count <= 0)
            return;
        if (Tasks.Count <= 0)
            return;

        for (int i = 0; i < AvailableWorker.Count; i++)
        {
            UpdateAvailableTasks();
            Debug.Log($"availableTasks.Count = {availableTasks.Count}");
            if (availableTasks.Find(task => AvailableWorker[i].TrySetTask(task)) != null)
            {
                AvailableWorker.Remove(AvailableWorker[i]);
            }
        }
    }

    List<ITask> availableTasks = new();
    public List<ITask> UpdateAvailableTasks()
    {
        availableTasks.Clear();
        Tasks.ForEach(task => task.GetSubTasks(availableTasks));
        return availableTasks;
    }
}
