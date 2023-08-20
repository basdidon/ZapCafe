using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOverlay : MonoBehaviour
{
    BoardManager BoardManager => BoardManager.Instance;
    WorkStationRegistry WorkStationRegistry => WorkStationRegistry.Instance;

    public static TileOverlay Instance { get; private set; }
    Tilemap TargetTilemap => BoardManager.Instance.WorkerArea;
    [field: SerializeField] public List<GameObject> ActivatedOverlayTile { get; set; }

    [field:SerializeField]public Sprite BlankSpaceOverlaySprite { get; private set; }
    [field:SerializeField]public Sprite WorkingCellOverlaySprite { get; private set; }

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
        //BlankSpacePos.Clear();
        area = TargetTilemap.cellBounds;

        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                var _cell = new Vector3Int(x, y, 0);
                if (TargetTilemap.HasTile(_cell))// && !BoardManager.IsBlankCell(_cell)) 
                {
                    if (ObjectPool.Instance.TryGetPoolObject("TileOverlay", out GameObject poolObject))
                    {
                        poolObject.SetActive(true);
                        poolObject.transform.position = TargetTilemap.GetCellCenterWorld(_cell);
                        ActivatedOverlayTile.Add(poolObject);

                        if(poolObject.TryGetComponent(out SpriteRenderer spriteRenderer))
                        {
                            if (WorkStationRegistry.IsWorkingCell(_cell))
                            {
                                spriteRenderer.sprite = WorkingCellOverlaySprite;
                            }
                            else
                            {
                                spriteRenderer.sprite = BlankSpaceOverlaySprite;
                                //BlankSpacePos.Add(_cell);
                            }
                        }

                    }
                }
            }
        }
    }

    public void Active()
    {
        Reset();
    }
    public void Deactive() => ActivatedOverlayTile.ForEach(tile => tile.SetActive(false));
}
