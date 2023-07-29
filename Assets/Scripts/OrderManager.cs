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
/*
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
            nextTask.Performed += delegate { Worker.Tasks.Remove(nextTask); };
            Worker.Tasks.Add(nextTask);
        };
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        return WorkStationRegistry.Instance.GetWorkStations(ItemData.WorkStation).ReadyToUse().FindClosest(worker);
    }
}*/

public class AddItemTo : Task
{
    public override float Duration => .5f;
    public ItemData ItemData { get; private set; }
    public WorkStationData WorkStationData { get; private set; }

    public AddItemTo(ItemData itemData,WorkStationData workStationData)
    {
        ItemData = itemData;
        WorkStationData = workStationData;
        /*
        Performed += delegate
        {
            (WorkStation as ItemFactory).Items.Add(Worker.HoldingItem);
            Worker.HoldingItem = null;
        };*/
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        return WorkStationRegistry.Instance.GetWorkStations(WorkStationData).ReadyToUse().FindClosest(worker);
    }
}

// need to done all subtask before run this task
public class GetItem : Task
{
    public override float Duration => 5f;
    public Order Order { get; private set; }
    public List<ItemData> Ingredients => Order.Menu.Ingredients;
    public ItemData ItemData { get; private set; }
    public WorkStationData WorkStationData => ItemData.WorkStation;
    public List<Task> Tasks { get; private set; }

    public GetItem(Order order,ItemData itemData)
    {
        Order = order;
        ItemData = itemData;
        Performed += delegate { (WorkStation as ItemFactory).CreateItem(itemData, Worker); };
        
        if(Ingredients.Count > 0)
        {
            Task[] conditionTasks = new Task[Ingredients.Count];
            for (int i = 0; i < Ingredients.Count; i++)
            {
                conditionTasks[i] = new AddItemTo(Ingredients[i], ItemData.WorkStation);
                TaskManager.Instance.AddTask(new GetItem(Order, Ingredients[i], conditionTasks[i]));
            }
            Tasks = new(conditionTasks);
        }
    }

    public GetItem(Order order, ItemData itemData,Task nextTask)//:this(order,itemData)
    {
        Order = order;
        ItemData = itemData;
        Performed += delegate { 
            (WorkStation as ItemFactory).CreateItem(itemData, Worker);
            nextTask.Performed += delegate { Worker.Tasks.Remove(nextTask); };
            Worker.Tasks.Add(nextTask);
        };

        if (Ingredients.Count > 0)
        {
            Task[] conditionTasks = new Task[Ingredients.Count];
            for (int i = 0; i < Ingredients.Count; i++)
            {
                conditionTasks[i] = new AddItemTo(Ingredients[i], ItemData.WorkStation);
                TaskManager.Instance.AddTask(new GetItem(Order, Ingredients[i], conditionTasks[i]));
            }
            Tasks = new(conditionTasks);
        }
        /*
        Performed += delegate {
            nextTask.Performed += delegate { Worker.Tasks.Remove(nextTask); };
            Worker.Tasks.Add(nextTask);
        };*/
    }

    public override IWorkStation GetworkStation(Worker worker)
    {
        if (Tasks == null || Tasks.Count==0 || IsAllFulfilled)
        {
            return WorkStationRegistry.Instance.GetWorkStations(WorkStationData).ReadyToUse().FindClosest(worker);
        }

        return null;
    }

    bool IsAllFulfilled => Tasks.TrueForAll(task => task.TaskState == TaskStates.Fulfilled);
}