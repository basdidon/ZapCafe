using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
/*
public interface ITask<out T> where T : Item {
    public Customer Customer { get; }
    public Worker Worker { get; set; }
    public IWorkStation<Item> WorkStation { get; set; }

    float Duration { get; }

    IWorkStation<T> GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
}
*/
public interface ITask
{
    //public Customer Customer { get; }
    //public Order Order { get; }
    public Worker Worker { get; set; }
    public IWorkStation WorkStation { get; set; }

    float Duration { get; }

    IWorkStation GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
}

public abstract class Task : ITask
{
    public Worker Worker { get; set; }
    [OdinSerialize] IWorkStation workStation;
    public IWorkStation WorkStation
    {
        get => workStation;
        set
        {
            workStation = value;
            Started += delegate {
                WorkStation.Worker = Worker;
                TaskState = TaskStates.Pending;
            };
            Performed += delegate {
                WorkStation.Worker = null;
                TaskState = TaskStates.Fulfilled;
            };
        }
    }

    public Task()
    {
        Performed += delegate {
            TaskManager.Instance.Tasks.Remove(this);
        };
    }

    public abstract float Duration { get; }

    public abstract IWorkStation GetworkStation(Worker worker);

    public Action Started { get; set; }
    public Action Performed { get; set; }
    public enum TaskStates { Created, Pending, Fulfilled }
    public TaskStates TaskState { get; private set; }
}

[Serializable]
public class Order
{
    [field:SerializeField] public Customer OrderBy { get; set; }
    [field:SerializeField] public Menu Menu { get; set; }
    public enum OrderStates { Created,Pending,Fullfill}
    public OrderStates OrderState;
    [field:SerializeField] public List<Item> Items { get; set; }

    public Order(Customer customer)
    {
        OrderBy = customer;
    }

    public void CreateMenu(ItemData itemData)
    {
        Menu = new Menu(this, itemData);
    }
}

[Serializable]
public class Menu
{
    public Order Order { get; }
    [ReadOnly]
    [field:SerializeField] 
    public ItemData ItemData { get;private set; }
    [field:SerializeField] public List<ItemData> Ingredients { get;private set; }
    public Menu(Order order,ItemData itemData)
    {
        Order = order;
        ItemData = itemData;
        Ingredients = ItemData.RequiredIngredients;
    }
    
    public Menu(Order order,ItemData itemData, List<ItemData> addOns) : this(order,itemData)
    {
        addOns.ForEach(addOn => {
            if (itemData.OptionalIngredients.Contains(addOn)) Ingredients.Add(addOn);
        });
    }

    public void OnStartCooking()
    {
        TaskManager.Instance.AddTask(new GetItem(Order, ItemData, new ServeOrderTask(Order.OrderBy)));
    }
}