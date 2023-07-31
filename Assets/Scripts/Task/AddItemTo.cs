using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                itemFactory.Items.Add(Worker.HoldingItem);
                Worker.HoldingItem = null;
            }
        };
        
        PrepareTasks = new ITask[] { new GetItemInverse(ItemData) };
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        return NextTask.TryGetWorkStation(worker, out workStation);
    }
}
