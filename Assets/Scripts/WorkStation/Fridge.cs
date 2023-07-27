using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : ItemFactory
{
    protected override void Start()
    {
        base.Start();
        WorkStationRegistry.Instance.AddWorkStation(this);
        TaskManager.Instance.WorkStationFree();
    }
}
