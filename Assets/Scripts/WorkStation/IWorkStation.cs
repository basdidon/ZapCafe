using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public interface IWorkStation : IBoardObject
{
    Worker Worker { get; set; }
    public bool IsAvailable { get => TaskManager.Instance.Tasks.Find(task => task.WorkStation == this) == null; }
    Vector3Int WorkingCell { get; }
    WorkStationData WorkStationData { get; }
}