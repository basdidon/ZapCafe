using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public interface IBoardObject
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }

    public Vector3Int CellPosition { get; }
    public Vector3 CellCenterWorld { get; }
}

public class BoardObject : SerializedMonoBehaviour,IBoardObject
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }

    public Vector3Int CellPosition { get => BoardManager.GetCellPos(transform.position); }
    public Vector3 CellCenterWorld { get => BoardManager.GetCellCenterWorld(CellPosition); }
}
