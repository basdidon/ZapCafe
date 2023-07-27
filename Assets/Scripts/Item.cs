using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public Sprite Sprite { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public float Price { get; set; }
    public float Time { get; set; }

    public Item(string name, Sprite sprite, float price, float time)
    {
        Name = name;
        Sprite = sprite;
        Price = price;
        Time = time;
    }
}