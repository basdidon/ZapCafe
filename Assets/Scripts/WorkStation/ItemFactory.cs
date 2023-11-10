using System.Collections.Generic;
using UnityEngine;

public sealed class ItemFactory : WorkStation
{
    [field: SerializeField] public List<ItemData> Recipes { get; set; }
    [field: SerializeField] public List<Item> Items { get; set; }
    [field: SerializeField] public List<Item> ToUsed { get; set; }

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
        Items = new();
        Recipes = new();
        ToUsed = new();
        foreach(var itemData in Resources.LoadAll<ItemData>("ItemDataSet"))
        {
            if(itemData.WorkStation == WorkStationData)
            {
                Recipes.Add(itemData);
            }
        }
        //UpdateFactoryData();
    }

    public void CreateItem(ItemData itemData,Worker worker)
    {
        var recipe = Recipes.Find(recipe => recipe.name == itemData.name);
        if (recipe != null)
        {
            //check ingredeints
            foreach(var requiredIngredeint in recipe.Ingredients)
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
