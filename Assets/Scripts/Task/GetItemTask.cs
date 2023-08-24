using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;

public class GetItemTask:BaseTask,IDependentTask
{
    public override float Duration => 5f;
    [field: SerializeField] public ItemData ItemData { get; private set; }
    public ReadOnlyCollection<ItemData> Ingredients => ItemData.Ingredients;
    public WorkStationData WorkStationData => ItemData.WorkStation;

    public ItemFactory ItemFactory { get; private set; }
    public ITask[] DependencyTasks { get; set; }

    public GetItemTask(ItemData itemData,int parentDepth):base(parentDepth)
    {
        //Debug.Log($"getItemtask:{itemData.name} was created");
        ItemData = itemData;

        Performed += delegate {
            if (WorkStation is ItemFactory itemFactory)
                itemFactory.CreateItem(ItemData, Worker);
        };

        if (WorkStationData == null)
            Debug.LogError("ItemData.WorkStationData Can't be null");

        if (Ingredients != null && Ingredients.Count > 0)
        {
            ITask[] tasks = Ingredients.Select((ingredient, idx) =>
            {
                var task = new AddItemToTask(this,Ingredients[idx],Depth);

                task.Pending += delegate
                {
                    if (WorkStation == null)
                        WorkStation = task.WorkStation;
                };
                return task;
            }).ToArray();

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

    public override IEnumerable<WorkerWorkStationPair> GetTaskCondition(IEnumerable<WorkerWorkStationPair> pairs)
    {
        return pairs.Where(pair => pair.Worker.HoldingItem == null);
    }

    public override bool TryCheckCondition(ref IEnumerable<WorkerWorkStationPair> pairs)
    {
        pairs = pairs.Where(pair => pair.Worker.HoldingItem == null);
        if (pairs.Count() > 0)
            return true;
        return false;
    }
}