using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using WorkerState;

public class Worker : Charecter
{
    [field: SerializeField] public Tilemap PathTilemap { get; set; }

    // task progress
    public GameObject TaskProgress;
    public Image ProgressImg;

    // Task
    [SerializeReference] ITask currentTask;
    public ITask CurrentTask
    {
        get => currentTask;
        set
        {
            currentTask = value;
            if(CurrentTask != null)
            {
                CurrentState = new MoveState(this, currentTask.Waypoints, new ExecutingTask(this));
                CurrentTask.Pending?.Invoke();
            }
        }
    }

    public bool TryGetWaypoint(Vector3Int targetCellPos,out List<Vector3Int> waypoints)
    {
        return PathFinder.TryFindWaypoint(this, CellPosition, targetCellPos, dirs, out waypoints);
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