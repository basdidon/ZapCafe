using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [field:SerializeField] public List<Order> Orders { get; private set; }
    public List<Worker> AvailableWorker { get; private set; }

    [Header("test")]
    public MenuData MenuData;
    public List<IngredientData> AddOns;

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

        // debug
        Orders = new() { new Order(null, new Menu(MenuData, AddOns)) };
    }


    public void AddOrder(Order newOrder)
    {
        if (newOrder == null)
            return;
        /*
        var worker = AvailableWorker.Find(worker => worker.TrySetTask(newTask));

        if (worker != null)
        {
            AvailableWorker.Remove(worker);
        }
        */
        Orders.Add(newOrder);
    }

    public void AddAvaliableWorker(Worker worker)
    {
        if (worker == null)
            return;
        /*
        var task = Tasks.Find(task => task.Worker == null && worker.TrySetTask(task));

        if (task == null)
        {
            AvailableWorker.Add(worker);
        }*/
    }

    public void WorkStationFree()
    {
        if (AvailableWorker.Count <= 0)
            return;

        var worker = AvailableWorker[0];/*
        var task = Tasks.Find(task => task.Worker == null && worker.TrySetTask(task));

        if (task == null)
        {
            AvailableWorker.Add(worker);
        }
        AvailableWorker.Remove(worker);
        */
    }
}
/*
public class GetItemOrder : Task
{
    public override float Duration => 1f;

    //public GetItemOrder(Order order) : base(order) { }

    public override IWorkStation GetworkStation(Worker worker)
    {
        //WorkStationRegistry.Instance.GetItemFactories
    }
}

/*
 * routine m_routine = new CookedOrder(new ServeOrder())
 * burger = 
 * 
 */