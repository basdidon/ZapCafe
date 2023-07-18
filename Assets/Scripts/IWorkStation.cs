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
    public int Level 
    { 
        get => level; 
        private set 
        {
            level = value;
            UpdateFactoryData();
        } 
    }

    public float Time { get; set; }
    public float Price { get; set; }
    public float Cost { get; set; }

    protected virtual void Start()
    {
        UpdateFactoryData();
    }

    protected void UpdateFactoryData()
    {
        ItemSO itemData = ItemList.Instance.GetItemData(ItemName);
        ItemLevel item = itemData.GetItemDataByLevel(Level);
        Time = item.time;
        Price = item.price;
        Cost = itemData.GetCostToUpgrade(Level);
    }

    public void UpLevel()
    {
        Debug.Log("up");
        var cost = ItemList.Instance.GetItemData(ItemName).GetCostToUpgrade(Level);
        if (cost <= LevelManager.Instance.Coin)
        {
            LevelManager.Instance.Coin -= cost;
            Level++;
            // update Ui
            ShowUiObject();
        }
    }


    public Item CreateItem() => ItemList.Instance.GetItemData(ItemName).CreateItem(Level);

    public void ShowUiObject()
    {
        var offset = transform.position + Vector3.up * 1.5f;
        UiObjectManager.Instance.DisplayFactoryPanel(offset,GetType().ToString(),Level,Time,Price,Cost);

        UiObjectManager.Instance.ClickUpgradeBtn = UpLevel;
    }
}