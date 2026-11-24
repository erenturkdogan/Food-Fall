using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public RecipeData recipe;
    public IngredientData[] possibleIngredients; // recipe dışı decoy'lar dahil
    public float spawnInterval = 1.5f;
    public float fallSpeed = 4f;
    public int timeLimitSeconds = 60;

    [Header("Endless Mode")]
    public bool isEndless = false;
    [Tooltip("Endless'ta tariflerin oluşturulacağı malzeme havuzu")]
    public IngredientData[] endlessPool;
    [Tooltip("Her tarifte en az / en çok kaç farklı malzeme")]
    public int endlessMinTypes = 2;
    public int endlessMaxTypes = 4;
    [Tooltip("Her malzemeden en fazla kaç adet istensin")]
    public int endlessMaxCount = 3;
    [Tooltip("Her tarif tamamlanınca verilecek bonus puan")]
    public int endlessRoundBonus = 500;
    [Tooltip("Her turda düşme hızına eklenecek miktar")]
    public float endlessSpeedStep = 0.3f;
    [Tooltip("Her turda spawn aralığından düşülecek miktar (min 0.4)")]
    public float endlessIntervalStep = 0.05f;
}
