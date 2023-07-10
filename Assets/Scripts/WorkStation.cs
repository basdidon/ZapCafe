using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public abstract class WorkStation : BoardObject
{
    [OdinSerialize]
    [BoxGroup("user")]
    public Worker Worker { get; set; }  // when someone use it
    public bool IsAvailable { get => TaskManager.Instance.Tasks.Find(task => task.WorkStation == this) == null; }
    public abstract Vector3Int WorkingCell { get; }
}
