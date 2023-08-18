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
    
    // WorkingCell
    bool isShowWorkingCells;
    public bool IsShowWorkingCells {
        get => isShowWorkingCells;
        set
        {
            isShowWorkingCells = value;
        } 
    }
    [field: SerializeField] public GameObject WorkingCellOverlayPrefab { get; set; }
    [field: SerializeField] public int WorkingCellOverlayPoolSize { get; set; }
    List<GameObject> WorkingCellOverlayPool { get; set; }

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
        for (int i = 0; i < WorkingCellOverlayPoolSize; i++)
        {
            var clone = Instantiate(WorkingCellOverlayPrefab,transform);
            clone.SetActive(false);
            WorkingCellOverlayPool.Add(clone);
        }
    }
    public void AddWorkStation(IWorkStation workStation) { workStations.Add(workStation); }
    public WorkStations GetWorkStations(WorkStationData workStationData) => workStations.Where(workStation => workStation.WorkStationData == workStationData);

    public IEnumerable<T> GetWorkStationsByType<T>() where T : IWorkStation
        => workStations.OfType<T>();

    // WorkStationCell
    IEnumerable<Vector3Int> WorkStationCells => workStations.SelectMany(workStation => workStation.WorldCellsPos);
    public bool IsWorkStationCell(Vector3Int cellPos) => WorkStationCells.Contains(cellPos);

    IEnumerable<Vector3Int> WorkingCells => workStations.Select(workStation => workStation.WorkingCell);
    public bool IsWorkingCell(Vector3Int cellPos) => WorkingCells.Contains(cellPos); 
}
