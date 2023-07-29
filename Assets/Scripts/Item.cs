using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public Sprite Sprite { get; set; }
    public string Name { get; set; }

    public Item(string name, Sprite sprite)
    {
        Name = name;
        Sprite = sprite;
    }
}