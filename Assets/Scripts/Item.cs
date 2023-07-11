using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public string Name { get; set; }
    public int Quantity { get; set; }

    public Item(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }
}

public class Donut : Item
{
    public Donut(string name, int quantity) : base(name, quantity) { }
}

public class Burger : Item
{
    public Burger(string name, int quatity) : base(name, quatity) { }
}
