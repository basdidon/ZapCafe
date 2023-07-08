using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BoardObject : SerializedMonoBehaviour
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }

    public Vector3Int CellPosition { get => BoardManager.GetCellPos(transform.position); }
    public Vector3 CellCenterWorld { get => BoardManager.GetCellCenterWorld(CellPosition); }
    /*
    private void Start()
    {

        if (!BoardManager.TryAddObject(this, BoardManager.GroundTileMap.WorldToCell(transform.position)))
        {
            Debug.LogError($"{this.name} add to { BoardManager.GroundTileMap.WorldToCell(transform.position)} was failed ");
        }  
    }*/
}
