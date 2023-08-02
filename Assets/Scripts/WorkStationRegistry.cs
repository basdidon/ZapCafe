using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class WorkStations : IEnumerable<IWorkStation>
{
    [SerializeField] List<IWorkStation> workStations;
    public int Count => workStations.Count;

    public WorkStations() { workStations = new(); }
    public WorkStations(List<IWorkStation> workStations) { this.workStations = workStations; }
    public WorkStations(IEnumerable<IWorkStation> workStations) { this.workStations = new(workStations);}

    public void Add(IWorkStation workStation) { workStations.Add(workStation); }
    public WorkStations Where(System.Func<IWorkStation,bool> match) => new(workStations.Where(match));

    public WorkStations ReadyToUse() => Where(workStation => workStation.IsAvailable);
    public IWorkStation FindClosest(BoardObject boardObject)
    {
        if (workStations == null || workStations.Count == 0)
        {
            return null;
        }

        var workStation = workStations[0];
        var minSqrMagnitude = workStation.SqrMagnitude(boardObject);

        for (int i = 1; i < workStations.Count; i++)
        {
            var sqrMagnitude = workStations[i].SqrMagnitude(boardObject);
            //Debug.Log($"{workStation.GetType()} = {minSqrMagnitude} : {workStations[i]} = {sqrMagnitude}"   );
            if (sqrMagnitude < minSqrMagnitude)
            {
                workStation = workStations[i];
                minSqrMagnitude = sqrMagnitude;
            }
        }
        //Debug.Log($" => {workStation.GetType()}");

        return workStation;
    }
    public IWorkStation First => workStations.Count > 0 ? workStations[0] : null;

    #region Implementation of IEnumerable
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<IWorkStation> GetEnumerator() => workStations.GetEnumerator();
    #endregion
}

/*
public class WorkStations : IEnumerable<IWorkStation<Item>>
{
    [SerializeField] List<IWorkStation<Item>> workStations;
    public int Count => workStations.Count;

    public WorkStations() { workStations = new(); }
    public WorkStations(List<IWorkStation<Item>> workStations) { this.workStations = workStations; }
    public WorkStations(IEnumerable<IWorkStation<Item>> workStations) { this.workStations = new(workStations); }

    public void Add(IWorkStation<Item> workStation) { workStations.Add(workStation); }
    public WorkStations Where(System.Func<IWorkStation<Item>, bool> match) => new(workStations.Where(match));
    public WorkStations FindAll(System.Predicate<IWorkStation<Item>> match) => new(workStations.FindAll(match));

    public IWorkStation<Item> FindClosest(BoardObject boardObject)
    {
        if (workStations == null || workStations.Count == 0) return null;

        var workStation = workStations[0];

        // *** Vector3.Distance(a,b) is the same as (a-b).magnitude ***
        // both method need to use square root for get the result
        // but in this function, distance is no matter
        // so i just use Vector3.sqrMagnitude find which object is closer

        float minSqrMagnitude = (boardObject.CellCenterWorld - workStation.CellCenterWorld).sqrMagnitude;
        for (int i = 1; i < workStations.Count; i++)
        {
            var sqrMagnitude = (boardObject.CellCenterWorld - workStations[i].CellCenterWorld).sqrMagnitude;
            if (sqrMagnitude < minSqrMagnitude)
            {
                workStation = workStations[i];
                minSqrMagnitude = sqrMagnitude;
            }
        }

        return workStation;
    }

    #region Implementation of IEnumerable
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<IWorkStation<Item>> GetEnumerator() => workStations.GetEnumerator();
    #endregion
}
*/
public class WorkStationRegistry : SerializedMonoBehaviour
{
    public static WorkStationRegistry Instance { get; private set; }

    [OdinSerialize] WorkStations workStations;

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

        workStations = new();
    }

    public void AddWorkStation(IWorkStation workStation) { workStations.Add(workStation); }

    public WorkStations GetWorkStations(WorkStationData workStationData) => workStations.Where(workStation => workStation.WorkStationData == workStationData);

    public IEnumerable<T> GetWorkStationsByType<T>() where T : IWorkStation
        => workStations.OfType<T>();
}
