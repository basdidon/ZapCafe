using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObject/Ingredient")]
public class IngredientData : ScriptableObject
{
    public Sprite sprite;

    // get
    public NewWorkstationData getFrom;
    public List<IngredientData> requiredIngredient;
}
