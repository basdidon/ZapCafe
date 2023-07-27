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

    public void Initialized(Bar bar,Tilemap pathTilemap)
    {
        Bar = bar;
        PathTilemap = pathTilemap;
        HoldingItem = null;

        var dirs = new List<Vector3Int>() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        if (PathFinder.TryFindWaypoint(this, BoardManager.Instance.GetCellPos(transform.position), Bar.ServiceCell, dirs, out List<Vector3Int> waypoints))
        {
            CurrentState = new CustomerMoveState(this, waypoints);
        }
        else
        {
            Debug.LogError($"can't move from {BoardManager.Instance.GetCellPos(transform.position)} to {Bar.ServiceCell}");
        }
    }

    // Monobehaviour
    protected void Awake()
    {
        IdleState = new CustomerIdleState();
        //CustomerOrders = new() { new GetItemTask(this,"Donut"), new GetItemTask(this,"Burger") };
        OrderSprite = null;
    }
    

    public override bool CanMoveTo(Vector3Int cellPos) => PathTilemap.HasTile(cellPos);

    //List<GetItemTask> CustomerOrders;
    public void GetOrder()
    {
        throw new System.NotImplementedException();
        /*
        var orderTask = CustomerOrders[Random.Range(0, CustomerOrders.Count)];
        TaskManager.Instance.AddTask(orderTask);

        OrderSprite = Resources.Load<ItemData>($"ItemDataSet/{orderTask.ItemName}").Sprite;
        */
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
                    var newTask = new GetOrderTask(Charecter, Charecter.Bar);
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
                //Debug.Log(Charecter.name);
                Charecter.gameObject.SetActive(false);
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


