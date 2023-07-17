using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerBox : ItemFactory
{
    public override string ItemName => "Burger";
    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
    }
}
