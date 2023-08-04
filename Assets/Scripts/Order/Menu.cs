using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections.ObjectModel;
/*
[Serializable]
public class Menu
{
    public Order Order { get; }
    [ReadOnly]
    [field: SerializeField]
    public ItemData ItemData { get; private set; }
    public ReadOnlyCollection<ItemData> Ingredients => ItemData.Ingredients;

    // Constructor
    public Menu(Order order, ItemData itemData)
    {
        Order = order;
        ItemData = itemData;
    }

    public bool OnStartCooking()
    {
        TaskManager.Instance.AddTask(new ServeOrderTaskInverse(Order, this, ItemData));
        return true;
    }
}*/