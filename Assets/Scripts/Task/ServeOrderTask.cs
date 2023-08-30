using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ServeOrderTask : BaseTask,IDependentTask
{
    Order Order { get; }
    Customer Customer => Order.OrderBy;
    [field: SerializeField] public ItemData ItemData { get; private set; }
    public IEnumerable<ITask> DependencyTasks { get; set; }

    public ServeOrderTask(Order order,ItemData itemData)
    {
        Order = order;
        ItemData = itemData;

        Performed += delegate {
            Customer.OrderSprite = null;
            Customer.HoldingItem = Worker.HoldingItem;
            Worker.HoldingItem = null;
            LevelManager.Instance.Coin += itemData.Price;
            LevelManager.Instance.Exp += 50;
            OrderManager.Instance.RemoveOrder(order);
            TextSpawner.Instance.SpawnText($"+ {itemData.Price}", Customer.transform.position + Vector3.up * 2);
            (WorkStation as Bar).CustomerLeave();
        };

        (this as IDependentTask).SetDependencyTasks(new GetItemTask(ItemData,Depth+1));
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

    public override bool TryCheckCondition(Worker worker, IWorkStation workStation) => worker.HoldingItem?.Name == ItemData.name;
}
