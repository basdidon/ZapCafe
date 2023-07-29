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
/*
public class GetDish : BaseTask
{
    public override float Duration => .5f;

    public override IWorkStation GetworkStation(Worker worker)
    {
        throw new System.NotImplementedException();
    }
}

public class Plating : BaseTask
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