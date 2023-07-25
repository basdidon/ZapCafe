using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOverlay : MonoBehaviour
{
    public static TileOverlay Instance { get; private set; }
    [SerializeField] Tilemap targetTilemap;
    [SerializeField] Tilemap overlayTilemap;
    [field:SerializeField] public List<Vector3Int> TilesPos { get; set; }

    public TileBase tileOverlay;
    public LayerMask blockConstructionMask;
    public BoundsInt area;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Deactive();
    }

    public void Reset()
    {
        overlayTilemap.ClearAllTiles();
        TilesPos.Clear();
        area = targetTilemap.cellBounds;

        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                var _cell = new Vector3Int(x, y, 0);
                if (targetTilemap.HasTile(_cell) && !Physics2D.Raycast(targetTilemap.GetCellCenterWorld(_cell), Vector2.down, .1f, blockConstructionMask.value))
                {
                    TilesPos.Add(_cell);
                }
            }
        }

        TilesPos.ForEach(tilePos => overlayTilemap.SetTile(tilePos, tileOverlay));
    }

    public void Active()
    {
        overlayTilemap.gameObject.SetActive(true);
        Reset();
    }
    public void Deactive() => overlayTilemap.gameObject.SetActive(false);
}
