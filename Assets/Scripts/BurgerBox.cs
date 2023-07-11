using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerBox : BoardObject, IWorkStation<Burger>
{
    // Worker
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    [SerializeField] Sprite sprite;
    public Sprite Sprite => sprite;

    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
    }

    public Burger GetItem()
    {
        throw new System.NotImplementedException();
    }
}
