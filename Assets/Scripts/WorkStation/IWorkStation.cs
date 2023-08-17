using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public interface IWorkStation : IBoardObject
{
    Worker Worker { get; set; }
    // WorkStationPos
    public Vector3Int[] LocalCellsPos { get; set; }
    public Vector3Int[] WorldCellsPos { get; }

    public bool IsAvailable { get => Worker == null; }

    // WorkingCell
    Vector3Int WorkingCellLocal { get; }
    Vector3Int WorkingCell => CellPosition + WorkingCellLocal;

    // Data
    WorkStationData WorkStationData { get; }

    // *** Vector3.Distance(a,b) is the same as (a-b).magnitude ***
    // both method need to use square root for get the result
    // but in this function, distance is no matter
    // so i just use Vector3.sqrMagnitude find which object is closer
    public float SqrMagnitude (BoardObject boardObject)=> (boardObject.CellCenterWorld - CellCenterWorld).sqrMagnitude;
}

public class WorkStation : BoardObject, IWorkStation
{
    public Worker Worker { get; set; }

    [SerializeField] Vector3Int[] localCellsPos = new Vector3Int[] { Vector3Int.zero };
    public Vector3Int[] LocalCellsPos
    {
        get => localCellsPos;
        set
        {
            localCellsPos = value;
        }
    }
    public Vector3Int[] WorldCellsPos => LocalCellsPos.Select(cell => CellPosition + cell).ToArray();

    [field: SerializeField] public Vector3Int WorkingCellLocal { get; set; }

    [field: SerializeField] public WorkStationData WorkStationData { get; private set; }

    private void Awake()
    {
        if (WorkStationData == null)
            Debug.LogWarning($"{gameObject.name} : WorkStationData is null");
    }
}