using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [field: SerializeField] public Grid MainGrid { get; set; }
    //[field: SerializeField] public Dic

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

        MainGrid = FindObjectOfType<Grid>();
    }

    // public bool IsFreeTile(Vector3Int cellPos) =>  GroundTileMap.HasTile(cellPos) && !ObjectsPosition.ContainsValue(cellPos);
    // GetWorldPosition
    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => MainGrid.GetCellCenterWorld(cellPos);
    // 
    public Vector3Int GetCellPos(Vector3 worldPos) => MainGrid.WorldToCell(worldPos);
}
