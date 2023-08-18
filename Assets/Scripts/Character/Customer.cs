using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CustomerState;

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

        if (PathFinder.TryFindWaypoint(this, BoardManager.Instance.GetCellPos(transform.position), Bar.ServiceCell, dirs, out List<Vector3Int> waypoints))
        {
            CurrentState = new MoveState(this, waypoints);
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
        IdleState = new IdleState();
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


