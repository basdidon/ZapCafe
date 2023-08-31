using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [field: SerializeField] public Sprite Sprite { get; set; }
    [field: SerializeField] public string Name { get; set; }

    public Item(string name, Sprite sprite)
    {
        Name = name;
        Sprite = sprite;
    }
}