using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }
    public Vector3Int CellPosition
    {
        get
        {
            if (!BoardManager.TryGetObjectPosition(this, out Vector3Int cellPos))
            {
                Debug.LogError($"{cellPos} not found in objects postion");
            }

            return cellPos;
        }

        set
        {
            if (BoardManager.IsFreeTile(value))
                BoardManager.ObjectsPosition[this] = value;
        }
    }

    public Vector3 CellCenterWorld { get => BoardManager.GetCellCenterWorld(CellPosition); }

    private void Start()
    {
        if (!BoardManager.TryAddObject(this, BoardManager.GroundTileMap.WorldToCell(transform.position)))
        {
            Debug.LogError($"{this.name} add to { BoardManager.GroundTileMap.WorldToCell(transform.position)} was failed ");
        }  
    }
}
