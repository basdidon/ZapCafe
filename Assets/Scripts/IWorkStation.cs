using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public interface IWorkStation : IBoardObject
{
    Worker Worker { get; set; }
    public bool IsAvailable { get => TaskManager.Instance.Tasks.Find(task => task.WorkStation == this) == null; }
    Vector3Int WorkingCell { get; }
}

public abstract class ItemFactory : BoardObject,IWorkStation,IUiObject
{
    // IWorkStation
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    public abstract string ItemName { get; }
    protected int level = 1;
    public int Level { get => level; }

    public void UpLevel()
    {
        var cost = ItemList.Instance.GetItemData(ItemName).GetCostToUpgrade(Level);
        if (cost < LevelManager.Instance.Coin)
        {
            LevelManager.Instance.Coin -= cost;
            level++;
        }
    }

    public Item CreateItem() => ItemList.Instance.GetItemData(ItemName).CreateItem(Level);

    public void ShowUiObject()
    {
        var offset = transform.position + Vector3.up * 1.5f;
        ItemLevel itemData = ItemList.Instance.GetItemData(ItemName).GetItemDataByLevel(Level);
        UiObjectManager.Instance.DisplayFactoryPanel(offset,GetType().ToString(),Level,itemData.time,itemData.price,itemData.cost);
    }

    public void HideUiObject()
    {
        UiObjectManager.Instance.HidePanel();
    }
}