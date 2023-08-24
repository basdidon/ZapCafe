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

    public void Add(IWorkStation workStation) => workStations.Add(workStation);
    public void Remove(IWorkStation workStation) => workStations.Remove(workStation);

    // Query
    public WorkStations Where(System.Func<IWorkStation,bool> match) => new(workStations.Where(match));
    public WorkStations ReadyToUse() { 
        var _workstations = Where(workStation => workStation.IsAvailable);
        Debug.Log($"ReadyToUse => {_workstations.Count}");
        return _workstations;
    }
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
            if (sqrMagnitude < minSqrMagnitude)
            {
                workStation = workStations[i];
                minSqrMagnitude = sqrMagnitude;
            }
        }

        return workStation;
    }
    public IWorkStation First => workStations.Count > 0 ? workStations[0] : null;

    #region Implementation of IEnumerable
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<IWorkStation> GetEnumerator() => workStations.GetEnumerator();
    #endregion
}

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

    // add & remove
    public void AddWorkStation(IWorkStation workStation) => workStations.Add(workStation);
    public void RemoveWorkStation(IWorkStation workStation) => workStations.Remove(workStation);

    // WorkStation Getter
    public WorkStations GetWorkStations(WorkStationData workStationData)
    {
        var _workstations = workStations.Where(workStation => workStation.WorkStationData == workStationData);
        Debug.Log($"GetWorkStations => {_workstations.Count}");
        return _workstations;
    }
    public IEnumerable<T> GetWorkStationsByType<T>() where T : IWorkStation
        => workStations.OfType<T>();

    // WorkStationCell
    IEnumerable<Vector3Int> WorkStationCells => workStations.SelectMany(workStation => workStation.WorldCellsPos);
    public bool IsWorkStationCell(Vector3Int cellPos) => WorkStationCells.Contains(cellPos);
    // WorkingCell
    IEnumerable<Vector3Int> WorkingCells => workStations.Select(workStation => workStation.WorkingCell);
    public bool IsWorkingCell(Vector3Int cellPos) => WorkingCells.Contains(cellPos); 
}
