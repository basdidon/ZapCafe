using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
/*
public interface IWorkStation<out T> : IBoardObject where T : Item
{
    Worker Worker { get; set; }
    public bool IsAvailable { get => TaskManager.Instance.Tasks.Find(task => task.WorkStation == this) == null; }
    Vector3Int WorkingCell { get; }

}*/

public interface IWorkStation : IBoardObject
{
    Worker Worker { get; set; }
    public bool IsAvailable { get => TaskManager.Instance.Tasks.Find(task => task.WorkStation == this) == null; }
    Vector3Int WorkingCell { get; }

}

public interface IItemFactory : IBoardObject
{
    string ItemName { get; }

    public Item CreateItem() => ItemList.Instance.GetItemData(ItemName).CreateItem();
}