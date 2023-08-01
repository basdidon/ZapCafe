using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public interface IWorkStation : IBoardObject
{
    Worker Worker { get; set; }
    public bool IsAvailable { get => Worker == null; }
    Vector3Int WorkingCell { get; }
    WorkStationData WorkStationData { get; }

    // *** Vector3.Distance(a,b) is the same as (a-b).magnitude ***
    // both method need to use square root for get the result
    // but in this function, distance is no matter
    // so i just use Vector3.sqrMagnitude find which object is closer
    public float SqrMagnitude (BoardObject boardObject)=> (boardObject.CellCenterWorld - CellCenterWorld).sqrMagnitude;
}