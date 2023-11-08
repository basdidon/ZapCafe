using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CustomerState;
using BasDidon.Direction;
using BasDidon.PathFinder;

public class Customer : Charecter
{
    [field: SerializeField] public Tilemap PathTilemap { get; set; }
    [field: SerializeField] public Bar Bar { get; set; }

    [field: SerializeField] GameObject textBubble;
    [field: SerializeField] SpriteRenderer OrderItemRenderer;
    Sprite orderSprite;
    public Sprite OrderSprite 
    {
        get => orderSprite;
        set
        {
            orderSprite = value;
            if(OrderSprite == null)
            {
                textBubble.SetActive(false);
            }
            else
            {
                OrderItemRenderer.sprite = OrderSprite;
                textBubble.SetActive(true);
            }
        }
    }


    public void Initialized(Bar bar,Tilemap pathTilemap)
    {
        Bar = bar;
        Bar.Customer = this;
        PathTilemap = pathTilemap;
        HoldingItem = null;

        if (GridPathFinder.TryFindPath(this, BoardManager.Instance.GetCellPos(transform.position), Bar.ServiceCell, DirectionGroup.Cardinal, out PathTraced pathTrace))
        {
            CurrentState = new MoveState(this, pathTrace.ToWayPoint());
        }
        else
        {
            Debug.LogError($"can't move from {BoardManager.Instance.GetCellPos(transform.position)} to {Bar.ServiceCell}");
        }
    }

    // Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        IdleState = new IdleState(this);
        OrderSprite = null;
    }

    public override bool CanMoveTo(Vector3Int cellPos) => PathTilemap.HasTile(cellPos);
    
    public List<ItemData> Orders;
    public void GetOrder()
    {
        var order = new Order(this);
        order.AddMenu(Orders[Random.Range(0, Orders.Count)]);
        OrderManager.Instance.AddOrder(order);
        OrderSprite = order.Menus[0].Sprite;
    }
}


