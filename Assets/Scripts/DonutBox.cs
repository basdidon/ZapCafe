using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class DonutBox : ItemFactory//,IUiObject
{
    public override string ItemName  => "Donut";

    protected override void Start()
    {
        base.Start();
        WorkStationRegistry.Instance.AddWorkStation(this);
    }

    public void BtnDebug()
    {
        Debug.Log("buttonHit");
        UpLevel();
    }
}

public class GetItem : Task
{
    public string ItemName { get; }
    public ItemFactory ItemFactory { get; set; }
    public override float Duration => ItemFactory.Time;

    public GetItem(Customer customer, string itemName) : base(customer)
    {
        ItemName = itemName;

        Performed += delegate {
            Worker.HoldingItem = (WorkStation as ItemFactory).CreateItem();
            var serveTask = new Bar.ServeOrderTask(customer);
            serveTask.Performed += delegate {
                Worker.HoldingItem = null;
                Worker.Tasks.Remove(serveTask);
            };
            Worker.Tasks.Add(serveTask);

            TaskManager.Instance.WorkStationFree();
        };
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        var result = WorkStationRegistry.Instance.GetItemFactories(ItemName).ReadyToUse().FindClosest(worker);
        ItemFactory = (ItemFactory) result;
        return result;
    }
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
