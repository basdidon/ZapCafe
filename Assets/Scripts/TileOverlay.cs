using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOverlay : MonoBehaviour
{
    public static TileOverlay Instance { get; private set; }
    [SerializeField] Tilemap targetTilemap;
    [field: SerializeField] public List<Vector3Int> TilesPos { get; set; }
    [field: SerializeField] public List<GameObject> ActivatedOverlayTile { get; set; }

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
        //overlayTilemap.ClearAllTiles();
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

        TilesPos.ForEach(tilePos => {
            if (ObjectPool.Instance.TryGetPoolObject("TileOverlay", out GameObject poolObject))
            {
                poolObject.SetActive(true);
                poolObject.transform.position = targetTilemap.GetCellCenterWorld(tilePos);
                ActivatedOverlayTile.Add(poolObject);
            }
        });
    }

    public void Active()
    {
        Reset();
    }
    public void Deactive() => ActivatedOverlayTile.ForEach(tile => tile.SetActive(false));
}
