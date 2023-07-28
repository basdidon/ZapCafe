using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : BoardObject,IWorkStation
{
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.Instance.GetCellPos(WorkingPoint.position); }
    public NewWorkstationData workstationData;

    protected void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
        TaskManager.Instance.WorkStationFree();
    }
}
