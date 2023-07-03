using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [field: SerializeField] public Tilemap GroundTileMap { get; set; }

    public Dictionary<BoardObject, Vector3Int> ObjectsPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        ObjectsPosition = new();
    }

    public bool IsFreeTile(Vector3Int cellPos) =>  GroundTileMap.HasTile(cellPos) && !ObjectsPosition.ContainsValue(cellPos);
    // GetWorldPosition
    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => GroundTileMap.GetCellCenterWorld(cellPos);
    // 
    public Vector3Int GetCellPos(Vector3 worldPos) => GroundTileMap.WorldToCell(worldPos);


    public bool TryAddObject(BoardObject boardObject, Vector3Int cellPos)
    {
        if (IsFreeTile(cellPos) && !ObjectsPosition.ContainsValue(cellPos))
        {
            ObjectsPosition.Add(boardObject, cellPos);
            Debug.Log($"{boardObject.name} added to {cellPos}");
            boardObject.transform.position = boardObject.CellCenterWorld;
            return true;
        }
        return false;
    }

    public bool TryGetObjectPosition(BoardObject boardObject, out Vector3Int cellPos)
    {
        cellPos = Vector3Int.zero;

        if (ObjectsPosition.ContainsKey(boardObject))
        {
            Debug.Log("yes");
            foreach (var objectPos in ObjectsPosition)
            {
                if (objectPos.Key == boardObject)
                {
                    cellPos = objectPos.Value;
                    return true;
                }
            }
        }
        return false;
    }


}
