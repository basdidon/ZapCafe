using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "ScriptableObject/Item")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; set; }
    public WorkStationData WorkStation;

    [BoxGroup("Ingredients")]
    public List<ItemData> RequiredIngredients;
    [BoxGroup("Ingredients")]
    public List<ItemData> OptionalIngredients;
}
