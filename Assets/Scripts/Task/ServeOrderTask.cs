using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class ServeOrderTask : BaseTask
{
    Customer Customer { get; }
    int price = 50;
    public ServeOrderTask(Customer customer) : base()
    {
        Customer = customer;
        Performed += delegate {
            Customer.OrderSprite = null;
            Customer.HoldingItem = Worker.HoldingItem;
            Worker.HoldingItem = null;
            Worker.Tasks.Remove(this);
            LevelManager.Instance.Coin += price;
            TextSpawner.Instance.SpawnText($"+ {price}", Customer.transform.position + Vector3.up * 2);
            (WorkStation as Bar).CustomerLeave();
        };
    }

    public override float Duration => 1f;

    public override IWorkStation GetworkStation(Worker worker)
    {
        foreach (Bar bar in WorkStationRegistry.Instance.GetWorkStationsByType<Bar>())
            if (bar.Customer == Customer)
                return bar;
        return null;
    }
}
*/

public class ServeOrderTaskInverse : BaseTask
{
    Menu Menu { get; }
    Customer Customer => Menu.Order.OrderBy;
    ItemData ItemData { get; }
    int price = 50;
    public ServeOrderTaskInverse(Menu menu,ItemData itemData)
    {
        Menu = menu;
        ItemData = itemData;
        PrepareTasks = new ITask[] { new GetItemInverse(ItemData) };
        Performed += delegate {
            Customer.OrderSprite = null;
            Customer.HoldingItem = Worker.HoldingItem;
            Worker.HoldingItem = null;
            Worker.Tasks.Remove(this);
            LevelManager.Instance.Coin += price;
            TextSpawner.Instance.SpawnText($"+ {price}", Customer.transform.position + Vector3.up * 2);
            (WorkStation as Bar).CustomerLeave();
        };
    }

    public override float Duration => 1f;

    public override bool TryGetWorkStation(Worker worker,out IWorkStation workStation)
    {
        workStation = null;
        foreach (Bar bar in WorkStationRegistry.Instance.GetWorkStationsByType<Bar>())
        {
            if (bar.Customer == Customer)
            {
                workStation = bar;
                return true;
            }
        }

        return false;
        
    }
}
