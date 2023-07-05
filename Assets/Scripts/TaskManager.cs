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
                worker.SetTask(newTask);
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
                worker.SetTask(task);
            }
            else
            {
                AvailableWorker.Enqueue(worker);
                worker.CurrentState = new WorkerIdle();
            }
        }
    }
}
