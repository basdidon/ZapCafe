using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using BasDidon.Direction;

public enum IsometricDirections
{
    NegativeX,  // left-down
    PositiveX,  // right-up
    NegativeY,  // right-down
    PositiveY,  // left-up
}

public interface IBoardObject
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }

    public Vector3Int CellPosition { get; }
    public Vector3 CellCenterWorld { get; }
}

public class BoardObject : MonoBehaviour, IBoardObject
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }

    //[OdinSerialize, OnValueChanged("OnDirectionChanged")]
    [SerializeField] Directions direction;
    public virtual Directions FacingDirection { get => direction; set => direction = value; }


    public Vector3Int CellPosition => BoardManager.GetCellPos(transform.position);
    public Vector3 CellCenterWorld => BoardManager.GetCellCenterWorld(CellPosition);
}
