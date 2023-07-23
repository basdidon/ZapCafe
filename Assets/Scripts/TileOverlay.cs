using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOverlay : MonoBehaviour
{
    public static TileOverlay Instance { get; private set; }
    [SerializeField] Tilemap targetTilemap;
    [SerializeField] Tilemap overlayTilemap;
    [SerializeField] List<Vector3Int> tilesPos;

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
        area = targetTilemap.cellBounds;

        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                var _cell = new Vector3Int(x, y, 0);
                if (targetTilemap.HasTile(_cell) && !Physics2D.Raycast(targetTilemap.GetCellCenterWorld(_cell), Vector2.down, .1f, blockConstructionMask.value))
                {
                    tilesPos.Add(_cell);
                }
            }
        }

        tilesPos.ForEach(tilePos => overlayTilemap.SetTile(tilePos, tileOverlay));
    }

    public void Active()
    {
        Reset();
        overlayTilemap.gameObject.SetActive(true);
    }
    public void Deactive() => overlayTilemap.gameObject.SetActive(false);
}
