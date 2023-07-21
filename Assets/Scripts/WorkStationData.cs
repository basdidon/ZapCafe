using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WorkstationData")]
public class WorkStationData : ScriptableObject
{
    public Sprite Sprite;
    public GameObject Prefab;
    public float Price;
}
