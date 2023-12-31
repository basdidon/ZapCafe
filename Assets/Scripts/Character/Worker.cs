using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using CharacterState.WorkerState;
using BasDidon.PathFinder;
using BasDidon.Direction;

public class Worker : Character
{
    [field: SerializeField] public Tilemap PathTilemap { get; set; }


    // task progress
    public GameObject TaskProgress;
    public Image ProgressImg;

    public bool TryGetWaypoint(Vector3Int targetCellPos,out List<Vector3Int> waypoints)
    {
        if(GridPathFinder.TryFindPath(this, CellPosition, targetCellPos, DirectionGroup.Cardinal, out PathTraced pathTraced))
        {
            waypoints = pathTraced.ToWayPoint();
            return true;
        }
        else
        {
            waypoints = null;
            return false;
        }
    }

    // State
    protected override void Awake()
    {
        base.Awake();
        IdleState = new IdleState(this);
    }

    protected override void Start()
    {
        base.Start();
        TaskProgress.SetActive(false);
    }

    public override bool CanMoveTo(Vector3Int cellPos)
    {
        return PathTilemap.HasTile(cellPos) && !WorkStationRegistry.Instance.IsWorkStationCell(cellPos);
    }
}