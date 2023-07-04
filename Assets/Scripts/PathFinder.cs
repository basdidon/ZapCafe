using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public interface IMoveable
    {
        public bool CanMoveTo(Vector3Int cellPos);
    }

    struct Node
    {
        public Vector3Int CellPosition { get; set; }
        public List<Vector3Int> Path { get; set; }
        public int G { get; set; }  // cumulative cost to this node, In this context mean ActionPoint
        public float H { get; set; }  // cost to targetCell,that ignore all obstacles
        public float F => G + H;

        public Node(Vector3Int cellPosition, int g, Vector3Int targetCell)
        {
            CellPosition = cellPosition;
            Path = new List<Vector3Int>();
            G = g;
            H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
        }

        public Node(Vector3Int cellPosition, int g, Vector3Int targetCell, List<Vector3Int> path)
        {
            CellPosition = cellPosition;
            Path = path;
            G = g;
            H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
        }
    }

    public static bool TryFindPath(IMoveable moveableObject,Vector3Int startCell, Vector3Int targetCell,List<Vector3Int> dirs, out List<Vector3Int> resultPath)
    {
        resultPath = new();

        if (!moveableObject.CanMoveTo(targetCell))
        {
            Debug.Log("target can't reach");
            return false;
        }

        Node startNode = new(startCell, 0, targetCell);
        var toSearch = new List<Node>() { startNode };
        var processed = new List<Node>();

        while (toSearch.Count > 0)
        {
            Debug.Log("Looping");
            Node currentNode = toSearch[0];
            foreach (var t in toSearch)
                if (t.F < currentNode.F || t.F == currentNode.F && t.H < currentNode.H)
                    currentNode = t;

            processed.Add(currentNode);
            toSearch.Remove(currentNode);

            foreach (var direction in dirs)
            {
                var nextPos = currentNode.CellPosition + direction;
                if (moveableObject.CanMoveTo(nextPos))
                {
                    var newPath = new List<Vector3Int>(currentNode.Path) { nextPos };

                    if (processed.Exists(cell => cell.CellPosition == nextPos))
                    {
                        var processedNode = processed.Find(cell => cell.CellPosition == nextPos);
                        // if new path use cost less than old node ,update that node
                        processedNode.G = currentNode.G + 1 < processedNode.G ? currentNode.G : processedNode.G;
                        processedNode.Path = newPath;
                    }
                    else
                    {
                        if (currentNode.CellPosition + direction == targetCell)
                        {
                            Debug.Log("found");
                            resultPath = newPath;
                            return true;
                        }
                        // add new node
                        toSearch.Add(new Node(currentNode.CellPosition + direction, currentNode.G + 1, targetCell, newPath));
                    }
                }
            }
        }

        Debug.Log("not found");
        return false;
    }
}