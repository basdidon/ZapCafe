using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IsometricDirections
{
    NegativeX,  // left-down
    PositiveX,  // right-up
    NegativeY,  // left-up
    PositiveY,  // right-down
}

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
    [field: SerializeField] public Vector3Int WorkingCellLocal { get; set; }

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
}
