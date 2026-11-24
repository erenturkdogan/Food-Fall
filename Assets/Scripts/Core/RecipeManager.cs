using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Aktif tarifi takip eder. Malzeme yakalandığında bu sınıf karar verir.
/// Endless modda tarif tamamlanınca yenisini üretir.
/// </summary>
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    private Dictionary<IngredientData, (int required, int current)> _state;
    private bool _recipeComplete = false;
    private bool _endless = false;

    public event System.Action<IngredientData, int, int> OnIngredientUpdated;
    public event System.Action OnRecipeComplete;       // normal level bitişi
    public event System.Action OnRecipeRegenerated;    // endless: yeni tarif geldi

    public int EndlessRound { get; private set; } = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        _state = new Dictionary<IngredientData, (int, int)>();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentLevel == null)
        {
            Debug.LogError("GameManager veya Level atanmamış!");
            return;
        }

        var level = GameManager.Instance.CurrentLevel;
        _endless = level.isEndless;

        if (_endless)
            GenerateEndlessRecipe();
        else
            InitRecipe(level.recipe);
    }

    public void InitRecipe(RecipeData recipe)
    {
        _state = new Dictionary<IngredientData, (int, int)>();
        _recipeComplete = false;

        foreach (var entry in recipe.entries)
            _state[entry.ingredient] = (entry.requiredCount, 0);

        foreach (var kv in _state)
            OnIngredientUpdated?.Invoke(kv.Key, 0, kv.Value.required);
    }

    /// <summary>Endless için havuzdan rastgele tarif üretir.</summary>
    public void GenerateEndlessRecipe()
    {
        var level = GameManager.Instance.CurrentLevel;
        var pool = new List<IngredientData>(level.endlessPool);
        _state = new Dictionary<IngredientData, (int, int)>();
        _recipeComplete = false;

        int typeCount = Random.Range(level.endlessMinTypes, level.endlessMaxTypes + 1);
        typeCount = Mathf.Min(typeCount, pool.Count);

        for (int i = 0; i < typeCount && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            var ing = pool[idx];
            pool.RemoveAt(idx); // aynı malzeme tekrar gelmesin
            int count = Random.Range(1, level.endlessMaxCount + 1);
            _state[ing] = (count, 0);
        }

        foreach (var kv in _state)
            OnIngredientUpdated?.Invoke(kv.Key, 0, kv.Value.required);

        OnRecipeRegenerated?.Invoke();
    }

    public bool CatchIngredient(IngredientData ingredient)
    {
        if (_recipeComplete) return false;
        if (!_state.ContainsKey(ingredient)) return false;

        var (required, current) = _state[ingredient];
        if (current >= required) return false;

        current++;
        _state[ingredient] = (required, current);
        OnIngredientUpdated?.Invoke(ingredient, current, required);

        CheckCompletion();
        return true;
    }

    private void CheckCompletion()
    {
        foreach (var kv in _state)
            if (kv.Value.current < kv.Value.required) return;

        _recipeComplete = true;

        if (_endless)
        {
            // Bonus puan + yeni tur
            EndlessRound++;
            var level = GameManager.Instance.CurrentLevel;
            GameManager.Instance.AddScore(level.endlessRoundBonus);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCatchCorrect();
            GenerateEndlessRecipe();
        }
        else
        {
            OnRecipeComplete?.Invoke();
        }
    }

    public bool IsComplete() => _recipeComplete;
    public bool IsEndless() => _endless;
    public Dictionary<IngredientData, (int required, int current)> GetState() => _state;
}
