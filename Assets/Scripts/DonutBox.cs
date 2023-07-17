using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class DonutBox : BoardObject,IWorkStation,IItemFactory,IUiObject
{
    // IWorkStation
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    public string ItemName => "Donut";
    [SerializeField] int level = 1;
    public int Level { get => level; set => level = value; }

    private void Awake()
    {
        //UiObjectManager.Instance.OnUiObjectActive += OnUiObjectActiveHandler;
    }

    public void OnUiObjectActiveHandler(IUiObject uiObject)
    {
        if (Equals(uiObject))
        {
            ShowUiObject();
        }
        else {
            HideUiObject();
        }
    }

    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
        UiObjectManager.Instance.AddUiObject(this);
        HideUiObject();
    }

    public void UpLevel()
    {
        var cost = ItemList.Instance.GetItemData(ItemName).GetCostToUpgrade(Level);
        if (cost < LevelManager.Instance.Coin)
        {
            LevelManager.Instance.Coin -= cost;
            Level++;
        }
    }

    public void BtnDebug()
    {
        Debug.Log("buttonHit");
        UpLevel();
    }

    public GameObject UiObject;
    public void ShowUiObject()
    {
        UiObject.SetActive(true);
    }

    public void HideUiObject()
    {
        UiObject.SetActive(false);
    }
}

public class GetItem : Task // <T> : Task<T> where T : Item
{
    public string ItemName { get; }
    public override float Duration => 3f;

    public GetItem(Customer customer, string itemName) : base(customer)
    {
        ItemName = itemName;

        Performed += delegate {
            Worker.HoldingItem = (WorkStation as IItemFactory).CreateItem();
            var serveTask = new Bar.ServeOrderTask(customer);
            serveTask.Performed += delegate {
                Worker.HoldingItem = null;
                Worker.Tasks.Remove(serveTask);
            };
            Worker.Tasks.Add(serveTask);

            TaskManager.Instance.WorkStationFree();
        };
    }

    public override IWorkStation GetworkStation(Worker worker) => WorkStationRegistry.Instance.GetItemFactories(ItemName).ReadyToUse().FindClosest(worker);


}

















    /*
    public override bool TryGetworkStation(Worker worker, out WorkStation workStation)
    {
        workStation = null;
        if (TaskManager.Instance.TryGetworkStation(worker, out DonutBox donutBox))
        {
            workStation = donutBox;
            return true;
        }

        return false;
    }
    /*
    public override float Duration => throw new System.NotImplementedException();

    public override bool TryGetworkStation(Worker worker, out WorkStation workStation)
    {
        throw new System.NotImplementedException();
    }*/
