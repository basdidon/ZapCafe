using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class BoardManager : MonoBehaviour
{
    WorkStationRegistry WorkStationRegistry => WorkStationRegistry.Instance;

    public static BoardManager Instance { get; private set; }

    [field: SerializeField] public Grid MainGrid { get; set; }

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

    // GetWorldPosition
    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => MainGrid.GetCellCenterWorld(cellPos);
    // CessPos
    public Vector3Int GetCellPos(Vector3 worldPos) => MainGrid.WorldToCell(worldPos);

    public bool IsBlankCell(Vector3Int cellPos) => !WorkStationRegistry.IsWorkStationCell(cellPos);
    public bool IsBuildableCell(Vector3Int cellPos) => !WorkStationRegistry.IsWorkStationCell(cellPos) && !WorkStationRegistry.IsWorkingCell(cellPos);

}
