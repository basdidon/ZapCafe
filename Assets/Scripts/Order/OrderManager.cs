using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        newOrder.StartMakingOrder();
    }
}