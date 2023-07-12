using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string Name { get; set; }
    public Sprite Sprite { get; set; }
    public int Quantity { get; set; }

    public Item(string name, Sprite sprite)
    {
        Name = name;
        Sprite = sprite;
    }

    public Item(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }
}

public class Burger : Item
{
    public Burger(string name, int quatity) : base(name, quatity) { }
}
