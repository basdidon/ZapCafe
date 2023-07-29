using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// need to done all subtask before run this task
public class GetItem : BaseTask
{
    public override float Duration => 5f;
    public Order Order { get; private set; }
    public List<ItemData> Ingredients => Order.Menu.Ingredients;
    [field:SerializeField] public ItemData ItemData { get; private set; }
    public WorkStationData WorkStationData => ItemData.WorkStation;
    [field: SerializeField] public BaseTask NextTask { get; private set; }

    public List<BaseTask> Tasks { get; private set; }
    public ItemFactory ItemFactory { get; private set; }

    public GetItem(Order order, ItemData itemData, BaseTask nextTask)
    {
        Order = order;
        ItemData = itemData;
        NextTask = nextTask;
        Performed += delegate {
            (WorkStation as ItemFactory).CreateItem(itemData, Worker);
            //nextTask.Performed += delegate { Worker.Tasks.Remove(nextTask); };
            //Worker.Tasks.Add(nextTask);
            nextTask.Worker = Worker;
            TaskManager.Instance.AddTask((ITask) nextTask);
            Debug.Log("getItem() done");
        };
    }

    public GetItem(Order order, ItemData itemData,ItemFactory itemFactory,BaseTask[] conditionTasks, BaseTask nextTask) : this(order, itemData, nextTask)
    {
        ItemFactory = itemFactory;
        Tasks = new(conditionTasks);
    }


    public override IWorkStation GetworkStation(Worker worker)
    {
        if (Tasks == null || Tasks.Count == 0)
        {
            return WorkStationRegistry.Instance.GetWorkStations(WorkStationData).ReadyToUse().FindClosest(worker);
        }
        else if(IsAllFulfilled)
        {
            return ItemFactory;
        }

        return null;
    }

    bool IsAllFulfilled => Tasks.TrueForAll(task => task.TaskState == TaskStates.Fulfilled);
}