using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Endless modda turlar ilerledikçe arka plan görselini değiştirir.
/// SpriteRenderer (dünya) veya UI Image bağlanabilir.
/// </summary>
public class EndlessBackgroundChanger : MonoBehaviour
{
    [Header("Hedef (birini doldur)")]
    public SpriteRenderer targetRenderer; // dünya arka planı
    public Image targetImage;             // UI arka planı

    [Header("Arka Plan Görselleri (sırayla)")]
    public Sprite[] backgrounds;

    [Tooltip("Kaç turda bir arka plan değişsin")]
    public int roundsPerBackground = 3;

    [Tooltip("Sırayla mı geçsin yoksa rastgele mi?")]
    public bool randomOrder = false;

    private int _currentIndex = 0;

    private bool _active = false;

    void Start()
    {
        // Sadece endless levelda çalış; normal levellarda arka plana dokunma
        bool isEndless = GameManager.Instance != null
            && GameManager.Instance.CurrentLevel != null
            && GameManager.Instance.CurrentLevel.isEndless;

        if (!isEndless) return;

        _active = true;
        if (RecipeManager.Instance != null)
            RecipeManager.Instance.OnRecipeRegenerated += OnRound;

        ApplyBackground(0);
    }

    void OnDestroy()
    {
        if (_active && RecipeManager.Instance != null)
            RecipeManager.Instance.OnRecipeRegenerated -= OnRound;
    }

    private void OnRound()
    {
        if (backgrounds == null || backgrounds.Length == 0) return;

        int round = RecipeManager.Instance.EndlessRound;
        int index;

        if (randomOrder)
            index = Random.Range(0, backgrounds.Length);
        else
            index = (round / Mathf.Max(1, roundsPerBackground)) % backgrounds.Length;

        if (index != _currentIndex)
            ApplyBackground(index);
    }

    private void ApplyBackground(int index)
    {
        if (backgrounds == null || index < 0 || index >= backgrounds.Length) return;
        _currentIndex = index;

        if (targetRenderer != null) targetRenderer.sprite = backgrounds[index];
        if (targetImage != null) targetImage.sprite = backgrounds[index];
    }
}
