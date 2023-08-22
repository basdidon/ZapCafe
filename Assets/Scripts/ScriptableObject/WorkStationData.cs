using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sprite4Dir
{
    public Sprite negativeX;
    public Sprite positiveX;
    public Sprite negativeY;
    public Sprite positiveY;
} 

[CreateAssetMenu(menuName = "ScriptableObject/WorkStation")]
public class WorkStationData : ScriptableObject
{
    [field: SerializeField] Sprite4Dir PreviewSprites { get; set; }
    [field: SerializeField] Sprite4Dir Sprites { get; set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public float Price { get; private set; }
    [SerializeField] Vector3Int[] localCellsPos = new Vector3Int[] { Vector3Int.zero };
    public Vector3Int[] LocalCellsPos
    {
        get => localCellsPos;
        set => localCellsPos = value;
    }

    [field: SerializeField] Vector3Int WorkingCellLocalDefault { get; set; }

    public Sprite GetSprite(IsometricDirections direction)
    {
        return direction switch
        {
            IsometricDirections.NegativeX => Sprites.negativeX,
            IsometricDirections.PositiveX => Sprites.positiveX,
            IsometricDirections.NegativeY => Sprites.negativeY,
            IsometricDirections.PositiveY => Sprites.positiveY,
            _ => null,
        };
    }

    public Sprite GetPreviewSprite(IsometricDirections direction)
    {
        return direction switch
        {
            IsometricDirections.NegativeX => PreviewSprites.negativeX,
            IsometricDirections.PositiveX => PreviewSprites.positiveX,
            IsometricDirections.NegativeY => PreviewSprites.negativeY,
            IsometricDirections.PositiveY => PreviewSprites.positiveY,
            _ => null,
        };
    }

    public Vector3Int GetWorkingCellLocal(IsometricDirections direction)
    {
        var cell = WorkingCellLocalDefault;
        return direction switch
        {
            IsometricDirections.NegativeX => new(-cell.y,cell.x,0),
            IsometricDirections.NegativeY => -cell,
            IsometricDirections.PositiveX => new(cell.y,-cell.x,0),
            IsometricDirections.PositiveY => cell,
            _ => Vector3Int.zero,
        };
    }
}
