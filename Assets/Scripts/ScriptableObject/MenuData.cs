using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Menu")]
public class MenuData : ScriptableObject
{
    public List<IngredientData> ingredients;
    public List<IngredientData> optionalIngredients;
}
