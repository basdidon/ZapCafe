using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServeOrderTask : Task
{
    Customer Customer { get; }
    public ServeOrderTask(Customer customer) : base()
    {
        Customer = customer;
        Performed += delegate {
            Customer.OrderSprite = null;
            Customer.HoldingItem = Worker.HoldingItem;
            Worker.HoldingItem = null;
            LevelManager.Instance.Coin += Customer.HoldingItem.Price;
            TextSpawner.Instance.SpawnText($"+ {Customer.HoldingItem.Price}", Customer.transform.position + Vector3.up * 2);
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
