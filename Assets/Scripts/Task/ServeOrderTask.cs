using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        var GetItemTask = new GetItemInverse(ItemData);
        GetItemTask.Performed += delegate { AssignWorker(GetItemTask.Worker); };
        PrepareTasks = new ITask[] { GetItemTask };

        Performed += delegate {
            Customer.OrderSprite = null;
            Customer.HoldingItem = Worker.HoldingItem;
            Worker.HoldingItem = null;
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

    public override IEnumerable<WorkerWorkStationPair> GetTaskCondition(IEnumerable<WorkerWorkStationPair> pairs)
    {
        return pairs.Where(pair => pair.Worker.HoldingItem.Name == ItemData.name);
    }

    public override bool TryCheckCondition(ref IEnumerable<WorkerWorkStationPair> pairs)
    {
        pairs = pairs.Where(pair => pair.Worker.HoldingItem?.Name == ItemData.name);
        if (pairs.Count() > 0)
            return true;
        return false;
    }
}
