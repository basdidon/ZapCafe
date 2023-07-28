using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [field:SerializeField] public List<Order> Orders { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddOrder(Order newOrder)
    {
        if (newOrder == null)
            return;

        Orders.Add(newOrder);
        newOrder.Menu.OnStartCooking();
    }
}

public class GetDish : Task
{
    public override float Duration => .5f;

    public override IWorkStation GetworkStation(Worker worker)
    {
        throw new System.NotImplementedException();
    }
}

public class Plating : Task
{
    public override float Duration => .5f;

    public override IWorkStation GetworkStation(Worker worker)
    {
        throw new System.NotImplementedException();
    }
}

public class GetItemOrder : Task
{
    [ReadOnly]
    [field:SerializeField] 
    public ItemData ItemData { get;private set; }
    public override float Duration => 1f;

    public GetItemOrder(ItemData itemData)
    {
        ItemData = itemData;
        Performed += delegate { (WorkStation as ItemFactory).CreateItem(itemData, Worker); };
    }

    public GetItemOrder(ItemData itemData, Task nextTask)
    {
        ItemData = itemData;
        Performed += delegate {
            (WorkStation as ItemFactory).CreateItem(itemData, Worker);
            Worker.Tasks.Add(nextTask);
        };
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        return WorkStationRegistry.Instance.GetWorkStations(ItemData.WorkStation).FindClosest(worker);
    }
}

public class AddItemTo : Task
{
    public override float Duration => .5f;
    public ItemData ItemData { get; private set; }
    public WorkStationData WorkStationData { get; private set; }

    public AddItemTo(ItemData itemData,WorkStationData workStationData)
    {
        ItemData = itemData;
        WorkStationData = workStationData;

        Performed += delegate
        {
            (WorkStation as ItemFactory).Items.Add(Worker.HoldingItem);
            Worker.HoldingItem = null;
        };
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        return WorkStationRegistry.Instance.GetWorkStations(WorkStationData).ReadyToUse().FindClosest(worker);
    }
}

// need to done all subtask before run this task
public class UseWorkStation : Task
{
    public override float Duration => 5f;
    public WorkStationData WorkStationData { get; private set; }
    public List<Task> Tasks { get; private set; }

    public UseWorkStation(WorkStationData workStationData,Task[] tasks)
    {
        WorkStationData = workStationData;
        Tasks = new(tasks);
        /*Tasks.ForEach(task => task.Performed += delegate {
            if (IsAllFulfilled) OnAllFulfilled();
        });*/
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        if (IsAllFulfilled)
        {
            return WorkStationRegistry.Instance.GetWorkStations(WorkStationData).ReadyToUse().FindClosest(worker);
        }

        return null;
    }

    bool IsAllFulfilled => Tasks.TrueForAll(task => task.TaskState == Task.TaskStates.Fulfilled);

    //private void OnAllFulfilled() => TaskManager.Instance.AddTask(this);
}