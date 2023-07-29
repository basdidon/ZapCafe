using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class Menu
{
    public Order Order { get; }
    [ReadOnly]
    [field: SerializeField]
    public ItemData ItemData { get; private set; }
    [field: SerializeField] public List<ItemData> Ingredients { get; private set; }
    public Menu(Order order, ItemData itemData)
    {
        Order = order;
        ItemData = itemData;
        Ingredients = ItemData.RequiredIngredients;
    }

    public Menu(Order order, ItemData itemData, List<ItemData> addOns) : this(order, itemData)
    {
        addOns.ForEach(addOn => { if (itemData.OptionalIngredients.Contains(addOn)) Ingredients.Add(addOn); });
    }

    public void OnStartCooking()
    {
        BaseTask mainTask = new ServeOrderTask(Order.OrderBy);

        HandleGetItem(mainTask,ItemData);
    }

    private void HandleGetItem(BaseTask nextTask,ItemData itemData)
    {
        var ingredients = itemData.RequiredIngredients;
        Debug.Log($"HandleGetItem({itemData.name}) : {ingredients.Count}");
        if (ingredients.Count == 0)
        {
            nextTask = new GetItem(Order, itemData, nextTask);
        }
        else
        {
            BaseTask[] conditionTasks = new BaseTask[ingredients.Count];
            if(WorkStationRegistry.Instance.GetWorkStations(ItemData.WorkStation).ReadyToUse().First is ItemFactory preparedItemFactory)
            {
                for (int i = 0; i < ingredients.Count; i++)
                {
                    conditionTasks[i] = new AddItemTo(ingredients[i], preparedItemFactory);
                    HandleGetItem(conditionTasks[i], ingredients[i]);
                }

                nextTask = new GetItem(Order, itemData,preparedItemFactory, conditionTasks, nextTask);
            }


        }

        TaskManager.Instance.AddTask(nextTask);
    }
}