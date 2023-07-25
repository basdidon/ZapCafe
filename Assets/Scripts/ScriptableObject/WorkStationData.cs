using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UpgradeWorkStationCostDict: UnitySerializedDictionary<int,float>{}

[CreateAssetMenu(menuName = "WorkstationData")]
public class WorkStationData : ScriptableObject
{
    public Sprite Sprite;
    public GameObject Prefab;
    public ItemData ItemData;
    public float Price;
    public UpgradeWorkStationCostDict upgradeCost;

    public float GetCostToUpgrade(int curLvl)
    {
        if (upgradeCost.TryGetValue(curLvl + 1, out float cost))
        {
            return cost;
        }
        else
        {
            throw new System.Exception("At max level");
        }
    }
}
