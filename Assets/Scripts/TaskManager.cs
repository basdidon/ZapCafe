using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    public Queue<ITask> Tasks { get; private set; }
    public Queue<Worker> AvailableWorker { get; private set; }

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

        Tasks = new Queue<ITask>();
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
                Tasks.Enqueue(newTask);
            }
        }
    }

    public void AddAvaliableWorker(Worker worker)
    {
        if(worker != null)
        {
            if (Tasks.TryDequeue(out ITask task))
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
