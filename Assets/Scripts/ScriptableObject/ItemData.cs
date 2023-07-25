using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ItemLevel
{
    public float time;
    public float price;
}

[Serializable]
public class ItemLevelDictionary : UnitySerializedDictionary<int, ItemLevel> { }

[CreateAssetMenu(menuName = "ScriptableObject/Item", fileName = "Item")]
public class ItemData : ScriptableObject 
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
    [SerializeReference] public ItemLevelDictionary itemLevelDict;

    public Item CreateItem(int level)
    {
        if(itemLevelDict.TryGetValue(level,out ItemLevel itemLevel)){
            return new Item(Name, Sprite, itemLevel.price , itemLevel.time);
        }
        else
        {
            throw new  System.Exception($"not found itemLevel at level {level}");
        }
    }

    public ItemLevel GetItemDataByLevel(int level)
    {
        if (itemLevelDict.TryGetValue(level, out ItemLevel itemLevel))
        {
            return itemLevel;
        }
        else
        {
            throw new System.Exception($"not found itemLevel at level {level}");
        }
    }
}
