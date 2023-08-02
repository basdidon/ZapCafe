using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
                itemFactory.Items.Add(Worker.HoldingItem);
                Worker.HoldingItem = null;
            }
        };
        var GetItemTask = new GetItemInverse(ItemData);
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
