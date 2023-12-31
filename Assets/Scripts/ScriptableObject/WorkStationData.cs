using UnityEngine;
using UnityEngine.Rendering;
using BasDidon.Direction;

[CreateAssetMenu(menuName = "ScriptableObject/WorkStation")]
public class WorkStationData : ScriptableObject
{
    [field: SerializeField] public SpriteDirection PreviewSprites { get;private set; }
    [field: SerializeField] public SpriteDirection Sprites { get;private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [SerializeField] Vector3Int[] localCellsPos = new Vector3Int[] { Vector3Int.zero };
    public Vector3Int[] LocalCellsPos
    {
        get => localCellsPos;
        set => localCellsPos = value;
    }

    [field: SerializeField] Vector3Int WorkingCellLocalDefault { get; set; }

    public void Instantiate(Vector3 position,Direction direction, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.position = position;
        go.transform.parent = parent;
        go.AddComponent<SpriteRenderer>();
        var sortingGroup = go.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "Object";

        if (name != "Bar")
        {
            var itemFactory = go.AddComponent<ItemFactory>();
            itemFactory.FacingDirection = direction;
            Initialize(itemFactory);
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    public void Initialize(IWorkStation workStation)
    {
        workStation.SpriteDirection = Sprites;
        workStation.WorkStationData = this;
    }
    
    public Vector3Int GetWorkingCellLocal(Direction direction)
    {
        var cell = WorkingCellLocalDefault;
        return direction switch
        {
            Direction.Left => new (-cell.y,cell.x,0),
            Direction.Down => -cell,
            Direction.Right => new(cell.y,-cell.x,0),
            _ => cell,
        };
    }
}
