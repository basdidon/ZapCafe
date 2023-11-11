using System.Collections.Generic;
using UnityEngine;

public sealed class ItemFactory : WorkStation
{
    [SerializeField] List<ItemData> recipes;
    public IReadOnlyList<ItemData> Recipes => recipes;
    [SerializeField] List<Item> items;
    public IReadOnlyList<Item> Items => items;
    [SerializeField] List<Item> toUsed;
    public IReadOnlyList<Item> ToUsed => toUsed;

    int level = 1;
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

    private void Start()
    {
        items = new();
        recipes = new();
        toUsed = new();
        foreach(var itemData in Resources.LoadAll<ItemData>("ItemDataSet"))
        {
            if(itemData.WorkStation == WorkStationData)
            {
                recipes.Add(itemData);
            }
        }
        //UpdateFactoryData();
    }

    public void AddItemFromWorker(Worker worker)
    {
        if (worker.HoldingItem == null)
            return;

        items.Add(worker.HoldingItem);
        worker.HoldingItem = null;
    }

    public void CreateItem(ItemData itemData,Worker worker)
    {
        var recipe = recipes.Find(recipe => recipe.name == itemData.name);
        if (recipe != null)
        {
            //check ingredeints
            foreach(var requiredIngredeint in recipe.Ingredients)
            {
                Debug.Log(requiredIngredeint.name);

                var _item = items.Find(item => item.Name == requiredIngredeint.name);
                if(_item != null)
                {
                    items.Remove(_item);
                    toUsed.Add(_item);
                }
                else
                {
                    items.AddRange(ToUsed);
                    toUsed.Clear();
                    Debug.LogError("ingredient not found");
                    return;
                }
            }

            toUsed.Clear();
            worker.HoldingItem = new Item(recipe.name, recipe.Sprite);
        }
        else
        {
            Debug.LogError("recipe not found");
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
