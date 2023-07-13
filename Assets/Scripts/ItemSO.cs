using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Item", fileName = "Item")]
public class ItemSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
    [field: SerializeField] public int Price { get; set; }

    public Item CreateItem()
    {
        return new Item(Name, Sprite, Price);
    }
}
