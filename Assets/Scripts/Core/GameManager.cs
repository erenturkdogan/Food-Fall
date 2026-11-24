using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Config")]
    public LevelData[] levels; // Inspector'dan level'lar atanır
    public int currentLevelIndex = 0;

    // MainMenu'dan hangi level'la başlanacağını taşımak için (GameManager henüz yokken set edilir)
    public static int StartLevelIndex = -1;

    [Header("Runtime State")]
    public int score = 0;
    public int combo = 0;
    public float quality = 100f;

    public LevelData CurrentLevel => levels[currentLevelIndex];

    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnComboChanged;
    public event System.Action<float> OnQualityChanged;
    public event System.Action OnLevelComplete;
    public event System.Action OnGameOver;

    private bool _gameEnded = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // MainMenu'dan bir başlangıç level'ı istendiyse uygula
        if (StartLevelIndex >= 0)
        {
            currentLevelIndex = StartLevelIndex;
            StartLevelIndex = -1;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void AddScore(int basePoints)
    {
        int multiplier = Mathf.Max(1, combo);
        score += basePoints * multiplier;
        OnScoreChanged?.Invoke(score);
    }

    public void IncreaseCombo()
    {
        combo++;
        OnComboChanged?.Invoke(combo);
    }

    public void ResetCombo()
    {
        if (combo == 0) return;
        combo = 0;
        OnComboChanged?.Invoke(combo);
    }

    public void ReduceQuality(float amount)
    {
        if (_gameEnded) return;
        quality = Mathf.Max(0f, quality - amount);
        OnQualityChanged?.Invoke(quality);
        if (quality <= 0f) TriggerGameOver();
    }

    public void IncreaseQuality(float amount)
    {
        if (_gameEnded) return;
        quality = Mathf.Min(100f, quality + amount);
        OnQualityChanged?.Invoke(quality);
    }

    public void TriggerLevelComplete()
    {
        if (_gameEnded) return;
        _gameEnded = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PauseMusic();
        OnLevelComplete?.Invoke();
    }

    public void TriggerGameOver()
    {
        if (_gameEnded) return;
        _gameEnded = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PauseMusic();
        OnGameOver?.Invoke();
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        ResetRuntimeState();
        if (currentLevelIndex < levels.Length)
            SceneManager.LoadScene("GameScene");
        else
            SceneManager.LoadScene("WinScene");
    }

    public void RestartLevel()
    {
        ResetRuntimeState();
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMainMenu()
    {
        currentLevelIndex = 0;
        ResetRuntimeState();
        SceneManager.LoadScene("MainMenu");
    }

    public void ResetRuntimeState()
    {
        // Score sahneler arası birikir, sadece game-over'da sıfırla
        combo = 0;
        quality = 100f;
        _gameEnded = false;

        // Eski sahnedeki subscriber'ları temizle (yeni sahnede yeniden bağlanacaklar)
        OnScoreChanged = null;
        OnComboChanged = null;
        OnQualityChanged = null;
        OnLevelComplete = null;
        OnGameOver = null;
    }

    public void FullReset()
    {
        score = 0;
        currentLevelIndex = 0;
        ResetRuntimeState();
    }
}
