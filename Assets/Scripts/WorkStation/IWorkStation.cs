using UnityEngine;
using System.Linq;

public interface IWorkStation : IBoardObject
{
    // Data
    WorkStationData WorkStationData { get; }
    //Worker Worker { get; set; }
    // WorkStationPos
    public Vector3Int[] LocalCellsPos { get; }
    public Vector3Int[] WorldCellsPos { get; }

    public bool IsAvailable => TaskManager.Instance.IsWorkstationAvailable(this);

    // WorkingCell
    Vector3Int WorkingCellLocal { get; }
    Vector3Int WorkingCell => CellPosition + WorkingCellLocal;

    // *** Vector3.Distance(a,b) is the same as (a-b).magnitude ***
    // both method need to use square root for get the result
    // but in this function, distance is no matter
    // so i just use Vector3.sqrMagnitude find which object is closer
    public float SqrMagnitude (BoardObject boardObject)=> (boardObject.CellCenterWorld - CellCenterWorld).sqrMagnitude;
}

public class WorkStation : BoardObject, IWorkStation
{
    // component
    SpriteRenderer SpriteRenderer { get; set; }

    [field: SerializeField] public WorkStationData WorkStationData { get; set; }

    public Vector3Int[] LocalCellsPos => WorkStationData.LocalCellsPos;
    public Vector3Int[] WorldCellsPos => LocalCellsPos.Select(cell => CellPosition + cell).ToArray();

    public Vector3Int WorkingCellLocal => WorkStationData.GetWorkingCellLocal(Direction);

    public override IsometricDirections Direction {
        get => base.Direction;
        set 
        { 
            base.Direction = value;
            var sprite = WorkStationData.GetSprite(Direction);

            if (sprite != null)
                SpriteRenderer.sprite = sprite;
        } 
    }

    // OdinCallback
    private void OnDirectionChanged()
    {
        var sprite = WorkStationData.GetSprite(Direction);
        if (sprite != null)
            if(TryGetComponent(out SpriteRenderer renderer))
                renderer.sprite = sprite;
    }

    public void Initialize(WorkStationData data, IsometricDirections dir)
    {
        if (TryGetComponent(out SpriteRenderer renderer))
        {
            SpriteRenderer = renderer;
            WorkStationData = data;
            Direction = dir;
        }
        else
        {
            Debug.LogError($"SpriteRenderer are required. ({gameObject.name})");
        }
    }

    private void Awake()
    {
        if (WorkStationData != null)
        {
            Initialize(WorkStationData, Direction);
        }
    }

    private void OnEnable()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
        TaskManager.Instance.TrySetTask();
    }
    private void OnDisable()
    {
        WorkStationRegistry.Instance.RemoveWorkStation(this);
        
        Debug.Log("OnDisable()");
    }
}