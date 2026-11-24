using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tarife göre runtime'da satır oluşturur (RecipeRowPrefab'tan).
/// Endless'ta tarif yenilenince satırları yeniden kurar.
/// </summary>
public class RecipeUI : MonoBehaviour
{
    [Header("Prefab & Container")]
    public RecipeRow recipeRowPrefab;
    public Transform rowContainer;

    private Dictionary<IngredientData, RecipeRow> _rows = new();

    void Start()
    {
        var rm = RecipeManager.Instance;
        rm.OnIngredientUpdated += UpdateRow;
        rm.OnRecipeRegenerated += RebuildRows;

        BuildRows();
    }

    void OnDestroy()
    {
        if (RecipeManager.Instance != null)
        {
            RecipeManager.Instance.OnIngredientUpdated -= UpdateRow;
            RecipeManager.Instance.OnRecipeRegenerated -= RebuildRows;
        }
    }

    private void BuildRows()
    {
        foreach (var kv in RecipeManager.Instance.GetState())
        {
            var row = Instantiate(recipeRowPrefab, rowContainer);
            row.Init(kv.Key, kv.Value.current, kv.Value.required);
            _rows[kv.Key] = row;
        }
    }

    private void RebuildRows()
    {
        // Eski satırları temizle
        foreach (var kv in _rows)
            if (kv.Value != null) Destroy(kv.Value.gameObject);
        _rows.Clear();

        BuildRows();
    }

    private void UpdateRow(IngredientData data, int current, int required)
    {
        if (_rows.TryGetValue(data, out var row))
            row.SetCount(current, required);
    }
}
