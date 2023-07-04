using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class Customer : BoardObject,PathFinder.IMoveable
{
    [field:SerializeField] public Tilemap PathTilemap { get; set; }
    [field:SerializeField] public List<Vector3Int> Path { get; set; }

    public bool CanMoveTo(Vector3Int cellPos)
    {
        Debug.Log(PathTilemap.HasTile(cellPos));
        return PathTilemap.HasTile(cellPos);
    }

    [Button]
    public void GetPath(Vector3Int startCell,Vector3Int targetCell)
    {
        if (PathFinder.TryFindPath(this,startCell,targetCell, out List<Vector3Int> path))
        {
            Path = path;
        }
    }

    public void GetOrder()
    {
        
    }
}
