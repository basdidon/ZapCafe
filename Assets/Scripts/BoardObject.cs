using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IBoardObject
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }

    public Vector3Int CellPosition { get; }
    public Vector3 CellCenterWorld { get; }
}

public class BoardObject : MonoBehaviour, IBoardObject
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }

    public Vector3Int CellPosition => BoardManager.GetCellPos(transform.position);
    public Vector3 CellCenterWorld => BoardManager.GetCellCenterWorld(CellPosition);
}
