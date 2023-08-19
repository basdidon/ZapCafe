using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/WorkStation")]
public class WorkStationData : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public float Price { get; private set; }
    [SerializeField] Vector3Int[] localCellsPos = new Vector3Int[] { Vector3Int.zero };
    public Vector3Int[] LocalCellsPos
    {
        get => localCellsPos;
        set => localCellsPos = value;
    }
    [field: SerializeField] public Vector3Int WorkingCellLocal { get; set; }

}
