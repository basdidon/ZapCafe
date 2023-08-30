using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AddItemToTask : BaseTask,IDependentTask
{
    public override float Duration => .5f;
    [field:SerializeField] public ItemData ItemData { get; private set; }
    public ITask NextTask { get; }
    public IEnumerable<ITask> DependencyTasks { get; set; }

    public AddItemToTask(ITask nextTask,ItemData itemData,int parentDepth):base(parentDepth)
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

        (this as IDependentTask).SetDependencyTasks(new GetItemTask(ItemData, Depth));
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        return NextTask.TryGetWorkStation(worker, out workStation);
    }

    public override bool TryCheckCondition(Worker worker, IWorkStation workStation) => worker.HoldingItem?.Name == ItemData.name;
}
