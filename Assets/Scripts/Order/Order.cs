using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Order
{
    [field: SerializeField] public Customer OrderBy { get; set; }
    [field: SerializeField] public List<ItemData> Menus { get; }

    public Order(Customer customer)
    {
        OrderBy = customer;
        Menus = new();
    }

    public void AddMenu(ItemData itemData)
    {
        Menus.Add(itemData);
    }

    public void StartMakingOrder()
    {
        Menus.ForEach(menu => TaskManager.Instance.AddTask(new ServeOrderTaskInverse(this,menu)));
    }
}
