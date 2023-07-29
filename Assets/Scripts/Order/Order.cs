using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Order
{
    [field: SerializeField] public Customer OrderBy { get; set; }
    [field: SerializeField] public Menu Menu { get; set; }
    public enum OrderStates { Created, Pending, Fullfill }
    public OrderStates OrderState;
    [field: SerializeField] public List<Item> Items { get; set; }

    public Order(Customer customer)
    {
        OrderBy = customer;
    }

    public void CreateMenu(ItemData itemData)
    {
        Menu = new Menu(this, itemData);
    }
}
