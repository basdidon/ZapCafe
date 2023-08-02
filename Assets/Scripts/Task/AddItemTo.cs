using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/*
public class AddItemTo : BaseTask
{
    public override float Duration => .5f;
    public ItemData ItemData { get; private set; }
    public ItemFactory ItemFactory { get; private set; }

    public AddItemTo(ItemData itemData, ItemFactory itemFactory)
    {
        ItemData = itemData;
        ItemFactory = itemFactory;
        Performed += delegate
        {
            if (WorkStation is ItemFactory itemFactory)
            {
                itemFactory.Items.Add(Worker.HoldingItem);
                Worker.HoldingItem = null;
            }
        };
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        return ItemFactory;
    }
}
*/
public class AddItemToInverse : BaseTask
{
    public override float Duration => .5f;
    [field:SerializeField] public ItemData ItemData { get; private set; }
    public ITask NextTask { get; }

    public AddItemToInverse(ITask nextTask,ItemData itemData)
    {
        NextTask = nextTask;
        ItemData = itemData;
        
        Performed += delegate
        {
            if (WorkStation is ItemFactory itemFactory)
            {
                var item = Worker.HoldingItem;
                if (itemFactory.Items.Contains(item))
                    Debug.LogWarning("adding same item");
                itemFactory.Items.Add(item);
                Worker.HoldingItem = null;
                Debug.Log($"{item.Name}({item.GetHashCode()}) added to {itemFactory.name} : {itemFactory.Items.Count}");
                Debug.Log($" - {itemFactory.Items.ToString()}");
                foreach(var _item in itemFactory.Items)
                {
                    Debug.Log(_item.GetHashCode());
                }
                Debug.Log("---------------------------");
            }
        };
        var GetItemTask = new GetItemInverse(ItemData);
        GetItemTask.Performed += delegate { AssignWorker(GetItemTask.Worker); };
        PrepareTasks = new ITask[] {GetItemTask};
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        return NextTask.TryGetWorkStation(worker, out workStation);
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
