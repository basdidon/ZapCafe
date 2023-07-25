using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class DonutBox : ItemFactory
{
    protected override void Start()
    {
        base.Start();
        WorkStationRegistry.Instance.AddWorkStation(this);
        TaskManager.Instance.WorkStationFree();
    }

    public void BtnDebug()
    {
        Debug.Log("buttonHit");
        UpLevel();
    }
}