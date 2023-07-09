using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class TaskManager : SerializedMonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [OdinSerialize] public List<Task> Tasks { get; private set; }
    [OdinSerialize] public List<Worker> AvailableWorker { get; private set; }
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

        Tasks = new();
        AvailableWorker = new();
    }

    public void AddTask(Task newTask)
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
        
        // TODO : loop to find task that can execute
        var task = Tasks.Find(task => worker.TrySetTask(task));

        if (task == null) 
        {
            AvailableWorker.Add(worker);
        }
        
    }

    public void AddTaskObject(TaskObject taskObject)
    {
        if (!TaskObjects.Contains(taskObject))
        {
            TaskObjects.Add(taskObject);
        }
    }

    [Button]
    public void CountBar() => Debug.Log(FindTaskObjectByType<Bar>().Count);
    public List<TaskObject> FindTaskObjectByType<T>()
    {
        return TaskObjects.FindAll(taskObject => (taskObject is T) && taskObject.IsAvailable);
    }

    public bool TryGetTaskObject<T>(BoardObject boardObject, out T taskObject) where T : TaskObject
    {
        taskObject = null;
        var taskObjects = TaskObjects.FindAll(taskObject => (taskObject is T) && taskObject.IsAvailable);

        if (taskObjects == null || taskObjects.Count == 0) return false;

        taskObject = (T) taskObjects[0];

        // *** Vector3.Distance(a,b) is the same as (a-b).magnitude ***
        // both method need to use square root for get the result
        // but in this function, distance is no matter
        // so i just use Vector3.sqrMagnitude find which object is closer
        float minSqrMagnitude = (boardObject.CellCenterWorld- taskObject.CellCenterWorld).sqrMagnitude;
        for(int i = 1; i < taskObjects.Count; i++)
        {
            var sqrMagnitude = (boardObject.CellCenterWorld - taskObjects[i].CellCenterWorld).sqrMagnitude;
            if ( sqrMagnitude < minSqrMagnitude)
            {
                taskObject = (T) taskObjects[i];
                minSqrMagnitude = sqrMagnitude;
            }
        }

        return true;
    }

    


}
