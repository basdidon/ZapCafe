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

    public bool OnStartCooking()
    {
        TaskManager.Instance.AddTask(new ServeOrderTaskInverse(this, ItemData));
        return true;
        /*
        BaseTask mainTask = new ServeOrderTask(Order.OrderBy);

        return TrySetTaskForMenu(mainTask,ItemData);*/
    }
    /*
    private bool TrySetTaskForMenu(BaseTask nextTask, ItemData itemData)
    {
        var ingredients = itemData.RequiredIngredients;
        //Debug.Log($"HandleGetItem({itemData.name}) : {ingredients.Count}");
        if (ingredients.Count == 0)
        {
            nextTask = new GetItem(Order, itemData, nextTask);
        }
        else
        {
            BaseTask[] conditionTasks = new BaseTask[ingredients.Count];
            if (WorkStationRegistry.Instance.GetWorkStations(ItemData.WorkStation).ReadyToUse().First is ItemFactory preparedItemFactory)
            {
                for (int i = 0; i < ingredients.Count; i++)
                {
                    conditionTasks[i] = new AddItemTo(ingredients[i], preparedItemFactory);
                    if(TrySetTaskForMenu(conditionTasks[i], ingredients[i]) == false)
                    {
                        return false;
                    }
                }

                nextTask = new GetItem(Order, itemData, preparedItemFactory, conditionTasks, nextTask);
            }
            else
            {
                return false;
            }


        }

        TaskManager.Instance.AddTask(nextTask);
        return true;
    }
    */
    /*
    private int TryGetPrepareTasks(ItemData itemData,out ITask[] PrepareTasks)
    {
        var ingredients = itemData.RequiredIngredients;

        if (ingredients.Count == 0)
        {
            PrepareTasks = new ITask[] { new GetItemInverse(Order, ItemData) };
        }
        else
        {
            ITask[] _addToTasks = new ITask[ingredients.Count];
            if (WorkStationRegistry.Instance.GetWorkStations(ItemData.WorkStation).ReadyToUse().First is ItemFactory preparedItemFactory)
            {
                for (int i = 0; i < ingredients.Count; i++)
                {
                    TryGetPrepareTasks(ingredients[i], out ITask[] _prepareTasks);
                    _addToTasks[i] = new AddItemToInverse(ingredients[i], preparedItemFactory,_prepareTasks);
                }

                //nextTask = new GetItem(Order, itemData, preparedItemFactory, conditionTasks, nextTask);
                PrepareTasks = new ITask[]{ new GetItemInverse(Order, itemData, preparedItemFactory, _addToTasks) };
            }
            else
            {
                PrepareTasks = null;
                return -1;
            }
        }
        
        TaskManager.Instance.AddTask(nextTask);
        return true;
        return PrepareTasks.Length;
    }*/
}