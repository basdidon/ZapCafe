using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemFactory : BoardObject, IWorkStation//, IUiObject
{
    // IWorkStation
    [field: SerializeField] public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    public string WorkStationName => $"{GetType()}";
    public WorkStationData WorkStationData { get; private set; }

    [field: SerializeField] public List<ItemData> Recipes { get; set; }
    [field: SerializeField] public ItemData WorkingMenu { get; set; }
    [field: SerializeField] public List<Item> Items { get; set; }
    [field: SerializeField] public List<Item> ToUsed { get; set; }

    protected int level = 1;
    public int Level
    {
        get => level;
        private set
        {
            level = value;
            //UpdateFactoryData();
        }
    }

    public float Time { get; set; }
    public float Price { get; set; }
    public float Cost { get; set; }

    protected virtual void Start()
    {
        Debug.Log(WorkStationName);
        WorkStationData = Resources.Load<WorkStationData>($"WorkStationDataSet/{WorkStationName}");
        if (WorkStationData == null)
        {
            Debug.LogError($"Resources.Load<WorkStationData>(WorkStationDataSet/{WorkStationName}) was failed.");
        }

        foreach(var itemData in Resources.LoadAll<ItemData>("ItemDataSet"))
        {
            if(itemData.WorkStation == WorkStationData)
            {
                Recipes.Add(itemData);
            }
        }
        //UpdateFactoryData();

        WorkStationRegistry.Instance.AddWorkStation(this);
        TaskManager.Instance.TrySetTask();
    }

    public void RequestItem(ItemData itemData)
    {
        WorkingMenu = itemData;
    }
    
    public void CreateItem(ItemData itemData,Worker worker)
    {
        Debug.Log("createItem");
        var recipe = Recipes.Find(recipe => recipe.name == itemData.name);
        if (recipe != null)
        {
            //check ingredeints
            foreach(var requiredIngredeint in recipe.RequiredIngredients)
            {
                Debug.Log(requiredIngredeint.name);

                var _item = Items.Find(item => item.Name == requiredIngredeint.name);
                if(_item != null)
                {
                    Items.Remove(_item);
                    ToUsed.Add(_item);
                }
                else
                {
                    Items.AddRange(ToUsed);
                    ToUsed.Clear();
                    Debug.LogError("ingredient not found");
                    return;
                }
            }

            ToUsed.Clear();
            worker.HoldingItem = new Item(recipe.name, recipe.Sprite);
        }
        else
        {
            Debug.LogError("item not found");
        }
    }

    /*
    protected void UpdateFactoryData()
    {

        
        ItemLevel item = ItemData.GetItemDataByLevel(level);
        Time = item.time;
        Price = item.price;
        Cost = WorkStationData.GetCostToUpgrade(level);
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

   //public Item CreateItem() => ItemData.CreateItem(Level);

    public void ShowUiObject()
    {
        var offset = transform.position + Vector3.up * 1.5f;
        UiObjectManager.Instance.DisplayFactoryPanel(offset, GetType().ToString(), Level, Time, Price, Cost);

        UiObjectManager.Instance.ClickUpgradeBtn = UpLevel;
    }*/
}
