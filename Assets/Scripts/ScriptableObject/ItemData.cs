using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections.ObjectModel;

[CreateAssetMenu(menuName = "ScriptableObject/Item")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public WorkStationData WorkStation { get; private set; }
    [field: SerializeField] public int Price { get; set; }

    [BoxGroup("Ingredients")]
    [field: SerializeField] List<ItemData> ingredients;
    public ReadOnlyCollection<ItemData> Ingredients => ingredients.AsReadOnly();
}
