using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GetItemInverse:BaseTask
{
    public override float Duration => .5f;
    [field: SerializeField] public ItemData ItemData { get; private set; }
    public WorkStationData WorkStationData => ItemData.WorkStation;
    [field: SerializeField] public BaseTask NextTask { get; private set; }

    public List<BaseTask> Tasks { get; private set; }
    public ItemFactory ItemFactory { get; private set; }

    public GetItemInverse(ItemData itemData)
    {
        //Debug.Log($"getItemtask:{itemData.name} was created");
        ItemData = itemData;

        Performed += delegate {
            if (WorkStation is ItemFactory itemFactory)
                itemFactory.CreateItem(itemData, Worker);
        };

        var ingredients = itemData.RequiredIngredients;
        if (ingredients != null && ingredients.Count > 0)
        {
            PrepareTasks = new ITask[ingredients.Count];
            for(int i =0;i<ingredients.Count;i++)
            {
                PrepareTasks[i] = new AddItemToInverse(this,ingredients[i]);
            }
        }
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        if(WorkStation != null)
        {
            workStation = WorkStation;
            return true;
        }

        workStation = WorkStationRegistry.Instance.GetWorkStations(ItemData.WorkStation).ReadyToUse().FindClosest(worker);
        if (workStation != null)
            return true;

        return false;
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