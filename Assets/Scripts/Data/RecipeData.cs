using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RecipeEntry
{
    public IngredientData ingredient;
    public int requiredCount;
}

[CreateAssetMenu(fileName = "RecipeData", menuName = "Game/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public List<RecipeEntry> entries;
}
