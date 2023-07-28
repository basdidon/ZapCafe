using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemTask : Task
{
    public ItemData ItemData { get; }
    public ItemFactory ItemFactory { get; set; }
    public override float Duration => ItemFactory.Time;

    public GetItemTask(Customer customer,ItemData itemData)
    {
        ItemData = itemData;

        /*
        Performed += delegate {
            Worker.HoldingItem = (WorkStation as ItemFactory).CreateItem();
            var serveTask = new ServeOrderTask(order);
            serveTask.Performed += delegate {
                Worker.HoldingItem = null;
                Worker.Tasks.Remove(serveTask);
            };
            Worker.Tasks.Add(serveTask);

            TaskManager.Instance.WorkStationFree();
        };*/
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        throw new System.NotImplementedException();
        /*
        var result = WorkStationRegistry.Instance.GetItemFactories(ItemName).ReadyToUse().FindClosest(worker);
        ItemFactory = (ItemFactory)result;
        return result;
        */
    }
}