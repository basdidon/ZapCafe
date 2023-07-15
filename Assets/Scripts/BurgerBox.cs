using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerBox : BoardObject, IWorkStation,IItemFactory
{
    // Worker
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    public string ItemName => "Burger";
    [SerializeField] int level = 1;
    public int Level { get => level; set => level = value; }

    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
    }
}
