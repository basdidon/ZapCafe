using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;

public class GetItemTask:BaseTask,IDependentTask
{
    public override float Duration => ItemData.BaseDuration;
    [field: SerializeField] public ItemData ItemData { get; private set; }
    public ReadOnlyCollection<ItemData> Ingredients => ItemData.Ingredients;
    public WorkStationData WorkStationData => ItemData.WorkStation;

    public ItemFactory ItemFactory { get; private set; }
    public IEnumerable<ITask> DependencyTasks { get; set; }

    public GetItemTask(ItemData itemData,int parentDepth):base(parentDepth)
    {
        ItemData = itemData;

        Performed += delegate {
            if (WorkStation is ItemFactory itemFactory)
                itemFactory.CreateItem(ItemData, Worker);
        };

        if (WorkStationData == null)
            Debug.LogError("ItemData.WorkStationData Can't be null");

        if (Ingredients != null && Ingredients.Count > 0)
        {
            ITask[] tasks = Ingredients.Select((ingredient, idx) => new AddItemToTask(this,Ingredients[idx],Depth)).ToArray();
            (this as IDependentTask).SetDependencyTasks(tasks);
        }
        else
        {
            TaskManager.Instance.AddTask(this);
        }
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        WorkStation ??= WorkStationRegistry.Instance.GetWorkStations(ItemData.WorkStation).ReadyToUse().FindClosest(worker);
        workStation = WorkStation;
        return workStation != null;
    }

    public override bool TryCheckCondition(Worker worker, IWorkStation workStation) => worker.HoldingItem == null;
}