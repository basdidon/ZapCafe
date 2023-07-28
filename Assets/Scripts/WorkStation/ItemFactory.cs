using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemFactory : BoardObject, IWorkStation, IUiObject
{
    // IWorkStation
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    public string WorkStationName => $"{GetType()}";
    public WorkStationData WorkStationData { get; private set; }
    public ItemData ItemData => WorkStationData.ItemData;

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
        Debug.Log(WorkStationName);
        UpdateFactoryData();
    }

    protected void UpdateFactoryData()
    {
        /*
        WorkStationData = Resources.Load<WorkStationData>($"WorkStationDataSet/{WorkStationName}");
        if (WorkStationData == null)
        {
            Debug.LogError($"Resources.Load<WorkStationData>(WorkStationDataSet/{WorkStationName}) was failed.");
        }
        ItemLevel item = ItemData.GetItemDataByLevel(level);
        Time = item.time;
        Price = item.price;
        Cost = WorkStationData.GetCostToUpgrade(level);*/
    }

    public void UpLevel()
    {
        if (LevelManager.Instance.TrySpend(Cost))
        {
            Level++;
            // update Ui
            ShowUiObject();
        }
    }


    public Item CreateItem() => ItemData.CreateItem(Level);

    public void ShowUiObject()
    {
        var offset = transform.position + Vector3.up * 1.5f;
        UiObjectManager.Instance.DisplayFactoryPanel(offset, GetType().ToString(), Level, Time, Price, Cost);

        UiObjectManager.Instance.ClickUpgradeBtn = UpLevel;
    }
}
