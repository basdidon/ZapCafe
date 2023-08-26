using UnityEngine;
using UnityEngine.Rendering;

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

    public void Instantiate(Vector3 position, Transform parent)
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
    
    public Vector3Int GetWorkingCellLocal(Directions direction)
    {
        var cell = WorkingCellLocalDefault;
        return direction switch
        {
            Directions.LeftDown => new (-cell.y,cell.x,0),
            Directions.RightDown => -cell,
            Directions.RightUp => new(cell.y,-cell.x,0),
            _ => cell,
        };
    }
}
