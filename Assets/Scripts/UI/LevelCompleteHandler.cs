using UnityEngine;

/// <summary>
/// Tarif tamamlandığında Level Complete'i tetikler.
/// GameManager ile RecipeManager arasındaki köprü.
/// </summary>
public class LevelCompleteHandler : MonoBehaviour
{
    void Start()
    {
        RecipeManager.Instance.OnRecipeComplete += HandleComplete;
    }

    void OnDestroy()
    {
        if (RecipeManager.Instance != null)
            RecipeManager.Instance.OnRecipeComplete -= HandleComplete;
    }

    private void HandleComplete()
    {
        Debug.Log("LevelCompleteHandler → TriggerLevelComplete çağrılıyor");
        if (GameManager.Instance != null)
            GameManager.Instance.TriggerLevelComplete();
        else
            Debug.LogError("GameManager.Instance null!");
    }
}
