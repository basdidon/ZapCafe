using UnityEngine;
using System.Linq;

public interface IWorkStation : IBoardObject
{
    // Data
    WorkStationData WorkStationData { get; set; }
    //Worker Worker { get; set; }
    // WorkStationPos
    public Vector3Int[] LocalCellsPos { get; }
    public Vector3Int[] WorldCellsPos { get; }

    public bool IsAvailable => TaskManager.Instance.IsWorkstationAvailable(this);

    // WorkingCell
    Vector3Int WorkingCellLocal { get; }
    Vector3Int WorkingCell => CellPosition + WorkingCellLocal;

    // sprite
    SpriteDirection SpriteDirection { get; set; }

    // *** Vector3.Distance(a,b) is the same as (a-b).magnitude ***
    // both method need to use square root for get the result
    // but in this function, distance is no matter
    // so i just use Vector3.sqrMagnitude find which object is closer
    public float SqrMagnitude (BoardObject boardObject)=> (boardObject.CellCenterWorld - CellCenterWorld).sqrMagnitude;
}

public class WorkStation : BoardObject, IWorkStation
{
    // component
    public SpriteRenderer SpriteRenderer { get; private set; }

    [field: SerializeField] public WorkStationData WorkStationData { get; set; }

    public Vector3Int[] LocalCellsPos => WorkStationData.LocalCellsPos;
    public Vector3Int[] WorldCellsPos => LocalCellsPos.Select(cell => CellPosition + cell).ToArray();

    public Vector3Int WorkingCellLocal => WorkStationData.GetWorkingCellLocal(Direction);

    public override Directions Direction { 
        get => base.Direction; 
        set
        {
            if (Direction == value)
                return;

            base.Direction = value;
            UpdateSprite();
        }
    }

    SpriteDirection spriteDirection;
    public SpriteDirection SpriteDirection {
        get => spriteDirection;
        set
        {
            spriteDirection = value;
            UpdateSprite();
        }
    }

    public void SetSpriteDirection(SpriteDirection _spriteDirection)
    {
        spriteDirection = _spriteDirection;
    }

    public void SetSpriteDirection(SpriteDirection spriteDirection, Directions direction)
    {
        SetSpriteDirection(spriteDirection);
        Direction = direction;
    }

    void UpdateSprite()
    {
        if (SpriteDirection == null)
            return;

        var sprite = SpriteDirection.GetSprite(Direction);
        if (sprite != null)
        {
            SpriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.Log("spriteDirection is null, but render default sprite instead.");
        }
    }

    // OdinCallback
    /*
    private void OnDirectionChanged()
    {
        var sprite = WorkStationData;
        if (sprite != null)
            if(TryGetComponent(out SpriteRenderer renderer))
                renderer.sprite = sprite;
    }*/

    private void Awake()
    {
        if (TryGetComponent(out SpriteRenderer renderer))
        {
            SpriteRenderer = renderer;
            if (WorkStationData != null)
            {
                WorkStationData.Initialize(this);
            }
        }
        else
        {
            Debug.LogError($"SpriteRenderer are required. ({gameObject.name})");
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
    }
}