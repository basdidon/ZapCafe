using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

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

    public void Initialized(Bar bar)
    {
        Bar = bar;
        PathTilemap = Bar.PathTile;

        var dirs = new List<Vector3Int>() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        if (PathFinder.TryFindWaypoint(this, Bar.SpawnCell, Bar.ServiceCell, dirs, out List<Vector3Int> waypoints))
        {
            CurrentState = new CustomerMoveState(this, waypoints);
        }
        else
        {
            Debug.LogError($"can't move from {Bar.SpawnCell} to {Bar.ServiceCell}");
        }
    }

    // Monobehaviour
    protected void Awake()
    {
        IdleState = new CustomerIdleState();
        CustomerOrders = new() { new GetItem(this,"Donut"), new GetItem(this,"Burger") };
        OrderSprite = null;
    }

    public override bool CanMoveTo(Vector3Int cellPos) => PathTilemap.HasTile(cellPos);

    List<GetItem> CustomerOrders;
    public void GetOrder()
    {
        Debug.Log("getOrder");
        var orderTask = CustomerOrders[Random.Range(0, CustomerOrders.Count)];
        TaskManager.Instance.AddTask(orderTask);

        OrderSprite = ItemList.Instance.GetItemSprite(orderTask.ItemName);
    }

    #region State
    public class CustomerIdleState : IState
    {
        public void EnterState(){}
        public void ExitState(){}
    }

    public class CustomerMoveState : MoveState<Customer>
    {
        public CustomerMoveState(Customer charecter,List<Vector3Int> waypoints):base(charecter,waypoints){}

        public override void SetNextState()
        {
            if (WayPoints.Count == 0)
            {
                if (Charecter.CellPosition == Charecter.Bar.ServiceCell)
                {
                    Charecter.Bar.Customer = Charecter;
                    var newTask = new Bar.GetOrderTask(Charecter, Charecter.Bar);
                    newTask.Performed += Charecter.GetOrder;
                    TaskManager.Instance.AddTask(newTask);
                }

                Charecter.CurrentState = Charecter.IdleState;
            }
            else
            {
                // self transition
                Charecter.CurrentState = new CustomerMoveState(Charecter,WayPoints);
            }
        }
    }

    public class CustomerExitState : MoveState<Customer>
    {
        public CustomerExitState(Customer charecter, List<Vector3Int> waypoints) : base(charecter, waypoints) { }

        public override void SetNextState()
        {
            if (WayPoints.Count == 0)
            {
                Debug.Log(Charecter.name);
                Destroy(Charecter.gameObject);
            }
            else
            {
                // self transition
                Charecter.CurrentState = new CustomerExitState(Charecter, WayPoints);
            }
        }
    }
    #endregion
}


