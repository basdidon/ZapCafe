using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    public static ItemList Instance { get; set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] List<ItemSO> Itemsdata;

    public Sprite GetItemSprite(string name)
    {
        return Itemsdata.Find(itemdata => itemdata.Name == name).Sprite;        
    }

    public ItemSO GetItemData(string name)
    {
        return Itemsdata.Find(itemdata => itemdata.Name == name);
    }
}
