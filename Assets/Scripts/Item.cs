using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string Name { get; set; }
    public Sprite Sprite { get; set; }
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

public class Burger : Item
{
    public Burger(string name, Sprite sprite, float price, float time) : base(name, sprite, price, time) { }
    /*
    public Burger(string name, int quatity) : base(name, quatity) { }*/
}
