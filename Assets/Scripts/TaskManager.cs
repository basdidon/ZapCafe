using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class TaskManager : SerializedMonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    [OdinSerialize] public List<ITask> Tasks { get; private set; }
    [OdinSerialize] public Queue<Worker> AvailableWorker { get; private set; }
    [OdinSerialize] public List<TaskObject> TaskObjects { get; set; }

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

        Tasks = new List<ITask>();
        AvailableWorker = new Queue<Worker>();
    }

    public void AddTasks(ITask newTask)
    {
        if (newTask != null)
        {
            if(AvailableWorker.TryDequeue(out Worker worker))
            {
                worker.Task = newTask;
            }
            else
            {
                Tasks.Add(newTask);
            }
        }
    }

    public void AddAvaliableWorker(Worker worker)
    {
        if(worker != null)
        {
            // TODO : loop to find task that can execute

            ITask task = Tasks.Find(task => task.CanExecute);

            if(task != null)
            {
                worker.Task = task;
                Tasks.Remove(task);
            }
            else
            {
                AvailableWorker.Enqueue(worker);
            }
        }
    }

    public void AddTaskObject(TaskObject taskObject)
    {
        if (!TaskObjects.Contains(taskObject))
        {
            TaskObjects.Add(taskObject);
        }
    }

    public List<TaskObject> FindTaskObjectByType<T>()
    {
        return TaskObjects.FindAll(taskObject => (taskObject is T) && (taskObject.Worker == null));
    }

    [Button]
    public void CountBar()
    {
        var result =  FindTaskObjectByType<Bar>();
        Debug.Log(result.Count);
    }
}
