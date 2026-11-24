using UnityEngine;

[CreateAssetMenu(fileName = "IngredientData", menuName = "Game/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public string ingredientName;
    public Sprite sprite;
    public bool isPoisonous; // zehirli malzeme
}
