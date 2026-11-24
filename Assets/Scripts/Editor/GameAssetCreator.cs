using UnityEngine;
using UnityEditor;

/// <summary>
/// Tüm IngredientData + 3 LevelData asset'ini otomatik oluşturur.
/// Tools > Setup Game Assets
/// Placeholder sprite'lar daha önce oluşturulmuş olmalı.
/// </summary>
public class GameAssetCreator : Editor
{
    private static readonly string SpritePath = "Assets/Sprites/Placeholders";
    // private static readonly string DataPath = "Assets/Data"; // reserved for future use

    [MenuItem("Tools/Setup Game Assets V2")]
    public static void SetupAll()
    {
        EnsureFolders();

        // --- Yiyecek asset'leri ---
        var avakado    = CreateIngredient("Avakado",    false);
        var banana     = CreateIngredient("Banana",     false);
        var carrot     = CreateIngredient("Carrot",     false);
        var cheese     = CreateIngredient("Cheese",     false);
        var chicken    = CreateIngredient("Chicken",    false);
        var cookie     = CreateIngredient("Cookie",     false);
        var corndog    = CreateIngredient("Corndog",    false);
        var cracker    = CreateIngredient("Cracker",    false);
        var egg        = CreateIngredient("Egg",        false);
        var fries      = CreateIngredient("Fries",      false);
        var hamburger  = CreateIngredient("Hamburger",  false);
        var hotdog     = CreateIngredient("Hotdog",     false);
        var icecream   = CreateIngredient("Icecream",   false);
        var mushroom   = CreateIngredient("Mushroom",   false);
        var pizza      = CreateIngredient("Pizza",      false);
        var sandwich   = CreateIngredient("Sandwich",   false);
        var cake       = CreateIngredient("Cake",       false);
        var steak      = CreateIngredient("Steak",      false);
        var sushi      = CreateIngredient("Sushi",      false);
        var tomato     = CreateIngredient("Tomato",     false);
        var watermelon = CreateIngredient("Watermelon", false);

        // 4 zehir
        var poison1 = CreateIngredient("Poison1", true);
        var poison2 = CreateIngredient("Poison2", true);
        var poison3 = CreateIngredient("Poison3", true);
        var poison4 = CreateIngredient("Poison4", true);

        // --- Tarif asset'leri ---
        // Level 1 — Burger menu
        var recipe1 = CreateRecipe("Recipe_Level1", new[]
        {
            (hamburger, 1),
            (fries,     1),
            (cheese,    1),
        });

        // Level 2 — Pizza partisi
        var recipe2 = CreateRecipe("Recipe_Level2", new[]
        {
            (pizza,    1),
            (mushroom, 2),
            (tomato,   1),
            (cheese,   1),
        });

        // Level 3 — Tatlı yemek karması
        var recipe3 = CreateRecipe("Recipe_Level3", new[]
        {
            (sushi,    1),
            (steak,    1),
            (icecream, 1),
            (cookie,   1),
            (cake,     1),
        });

        // --- Level asset'leri ---
        // Decoy = tarifte olmayan, oyuncuyu yanıltacak malzemeler (zehirler dahil)
        CreateLevel("Level1", recipe1,
            new IngredientData[] { hotdog, sandwich, sushi, banana, poison1 },
            spawnInterval: 1.7f, fallSpeed: 3.5f, timeLimit: 60);

        CreateLevel("Level2", recipe2,
            new IngredientData[] { hamburger, sandwich, hotdog, corndog, watermelon, poison1, poison2 },
            spawnInterval: 1.4f, fallSpeed: 4.5f, timeLimit: 60);

        CreateLevel("Level3", recipe3,
            new IngredientData[] { hamburger, pizza, fries, chicken, egg, cracker, avakado, poison1, poison2, poison3, poison4 },
            spawnInterval: 1.1f, fallSpeed: 5.5f, timeLimit: 70);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Yeni asset'ler oluşturuldu. Sprite'ları her Ingredient asset'ine atamayı unutma.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");
        if (!AssetDatabase.IsValidFolder("Assets/Data/Ingredients"))
            AssetDatabase.CreateFolder("Assets/Data", "Ingredients");
        if (!AssetDatabase.IsValidFolder("Assets/Data/Recipes"))
            AssetDatabase.CreateFolder("Assets/Data", "Recipes");
        if (!AssetDatabase.IsValidFolder("Assets/Data/Levels"))
            AssetDatabase.CreateFolder("Assets/Data", "Levels");
    }

    private static IngredientData CreateIngredient(string ingName, bool isPoisonous)
    {
        string path = $"Assets/Data/Ingredients/{ingName}.asset";
        var existing = AssetDatabase.LoadAssetAtPath<IngredientData>(path);
        if (existing != null) return existing;

        var data = ScriptableObject.CreateInstance<IngredientData>();
        data.ingredientName = ingName;
        data.isPoisonous = isPoisonous;
        data.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{SpritePath}/{ingName}.png");
        AssetDatabase.CreateAsset(data, path);
        return data;
    }

    private static RecipeData CreateRecipe(string recipeName, (IngredientData ing, int count)[] entries)
    {
        string path = $"Assets/Data/Recipes/{recipeName}.asset";
        var existing = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
        if (existing != null) return existing;

        var data = ScriptableObject.CreateInstance<RecipeData>();
        data.recipeName = recipeName;
        data.entries = new System.Collections.Generic.List<RecipeEntry>();
        foreach (var (ing, count) in entries)
            data.entries.Add(new RecipeEntry { ingredient = ing, requiredCount = count });

        AssetDatabase.CreateAsset(data, path);
        return data;
    }

    private static void CreateLevel(string levelName, RecipeData recipe,
        IngredientData[] decoys, float spawnInterval, float fallSpeed, int timeLimit)
    {
        string path = $"Assets/Data/Levels/{levelName}.asset";
        if (AssetDatabase.LoadAssetAtPath<LevelData>(path) != null) return;

        var data = ScriptableObject.CreateInstance<LevelData>();
        data.recipe = recipe;
        data.possibleIngredients = decoys;
        data.spawnInterval = spawnInterval;
        data.fallSpeed = fallSpeed;
        data.timeLimitSeconds = timeLimit;

        AssetDatabase.CreateAsset(data, path);
    }
}
