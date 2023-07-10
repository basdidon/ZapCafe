using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class TaskManager : SerializedMonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [OdinSerialize] public List<ITask<WorkStation>> Tasks { get; private set; }
    [OdinSerialize] public List<Worker> AvailableWorker { get; private set; }
    [OdinSerialize] public List<WorkStation> WorkStations { get; set; }

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

    public void AddTask(ITask<WorkStation> newTask)
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

    public bool TryGetTaskByworkStation<T>(out ITask<WorkStation> task) where T : WorkStation
    {
        task = Tasks.Find(task => task.Worker == null && task.WorkStation != null && task.WorkStation.GetType() == typeof(T));

        if (task == null)
            return false;
        return true;
    }

    // when 
    public void WorkStationFree<T>() where T:WorkStation
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

    public void AddworkStation(WorkStation workStation)
    {
        if (!WorkStations.Contains(workStation))
        {
            WorkStations.Add(workStation);
        }
    }


    [Button]
    public void CountBar() => Debug.Log(FindworkStationByType<Bar>().Count);
    public List<WorkStation> FindworkStationByType<T>()
    {
        return WorkStations.FindAll(workStation => (workStation is T) && workStation.IsAvailable);
    }

    public bool TryGetworkStation<T>(BoardObject boardObject, out T workStation) where T : WorkStation
    {
        workStation = null;
        var workStations = WorkStations.FindAll(workStation => (workStation is T) && workStation.IsAvailable);

        if (workStations == null || workStations.Count == 0) return false;

        workStation = (T) workStations[0];

        // *** Vector3.Distance(a,b) is the same as (a-b).magnitude ***
        // both method need to use square root for get the result
        // but in this function, distance is no matter
        // so i just use Vector3.sqrMagnitude find which object is closer
        float minSqrMagnitude = (boardObject.CellCenterWorld- workStation.CellCenterWorld).sqrMagnitude;
        for(int i = 1; i < workStations.Count; i++)
        {
            var sqrMagnitude = (boardObject.CellCenterWorld - workStations[i].CellCenterWorld).sqrMagnitude;
            if ( sqrMagnitude < minSqrMagnitude)
            {
                workStation = (T) workStations[i];
                minSqrMagnitude = sqrMagnitude;
            }
        }

        return true;
    }

    


}
