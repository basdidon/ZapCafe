using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [field: SerializeField] public Tilemap GroundTileMap { get; set; }

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

        //ObjectsPosition = new();
    }

    // public bool IsFreeTile(Vector3Int cellPos) =>  GroundTileMap.HasTile(cellPos) && !ObjectsPosition.ContainsValue(cellPos);
    // GetWorldPosition
    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => GroundTileMap.GetCellCenterWorld(cellPos);
    // 
    public Vector3Int GetCellPos(Vector3 worldPos) => GroundTileMap.WorldToCell(worldPos);
}
