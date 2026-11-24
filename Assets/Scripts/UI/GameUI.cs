using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public Slider qualitySlider;
    public TextMeshProUGUI qualityPercentText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI levelText;

    [Header("Panels")]
    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI levelCompleteScoreText;
    public TextMeshProUGUI gameOverScoreText;
    [Tooltip("Level Complete'teki 'Next Level' butonunun yazısı")]
    public TextMeshProUGUI nextLevelButtonText;

    void Start()
    {
        var gm = GameManager.Instance;
        Debug.Log($"<color=yellow>GameUI Start. GameManager null mu? {gm == null}</color>");
        if (gm == null) return;
        gm.OnScoreChanged   += UpdateScore;
        gm.OnComboChanged   += UpdateCombo;
        gm.OnQualityChanged += UpdateQuality;
        gm.OnLevelComplete  += ShowLevelComplete;
        gm.OnGameOver       += ShowGameOver;

        var timer = FindAnyObjectByType<TimerController>();
        if (timer != null) timer.OnTimerTick += UpdateTimer;

        // İlk değerleri yaz
        UpdateScore(gm.score);
        UpdateCombo(gm.combo);
        UpdateQuality(gm.quality);

        // Endless modda timer ∞ göster
        if (gm.CurrentLevel != null && gm.CurrentLevel.isEndless && timerText != null)
            timerText.text = "∞";

        if (levelText != null)
            levelText.text = gm.CurrentLevel != null && gm.CurrentLevel.isEndless
                ? "ENDLESS"
                : $"LEVEL {gm.currentLevelIndex + 1}";

        levelCompletePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged   -= UpdateScore;
        GameManager.Instance.OnComboChanged   -= UpdateCombo;
        GameManager.Instance.OnQualityChanged -= UpdateQuality;
        GameManager.Instance.OnLevelComplete  -= ShowLevelComplete;
        GameManager.Instance.OnGameOver       -= ShowGameOver;
    }

    private void UpdateScore(int val)   => scoreText.text = val.ToString("N0");

    private void UpdateCombo(int val)
    {
        if (comboText == null) return;
        comboText.text = val > 1 ? $"COMBO x{val}" : "";
        comboText.gameObject.SetActive(val > 1);
    }

    private void UpdateQuality(float val)
    {
        if (qualitySlider != null)
            qualitySlider.value = val / 100f;
        if (qualityPercentText != null)
            qualityPercentText.text = $"{Mathf.RoundToInt(val)}%";
    }

    private void UpdateTimer(float remaining)
    {
        if (timerText == null) return;
        // Endless modda timer'ı ∞ tut, güncelleme
        if (GameManager.Instance != null && GameManager.Instance.CurrentLevel != null
            && GameManager.Instance.CurrentLevel.isEndless)
        {
            timerText.text = "∞";
            timerText.color = Color.black;
            return;
        }
        int secs = Mathf.CeilToInt(Mathf.Max(0f, remaining));
        timerText.text = secs.ToString();
        timerText.color = secs <= 10 ? Color.red : Color.black;
    }

    private void ShowLevelComplete()
    {
        levelCompletePanel.SetActive(true);
        var gm = GameManager.Instance;
        if (levelCompleteScoreText != null)
            levelCompleteScoreText.text = gm.score.ToString("N0");

        // Buton, sıradaki levelın adını göstersin
        Debug.Log($"ShowLevelComplete. nextLevelButtonText null mu? {nextLevelButtonText == null}, currentIndex={gm.currentLevelIndex}");
        if (nextLevelButtonText != null)
        {
            int nextIndex = gm.currentLevelIndex + 1;
            if (nextIndex >= gm.levels.Length)
                nextLevelButtonText.text = "BİTİR";
            else if (gm.levels[nextIndex] != null && gm.levels[nextIndex].isEndless)
                nextLevelButtonText.text = "ENDLESS LEVEL";
            else
                nextLevelButtonText.text = $"LEVEL {nextIndex + 1}";
        }
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        if (gameOverScoreText != null)
            gameOverScoreText.text = GameManager.Instance.score.ToString("N0");
    }

    // UI butonlarından çağrılır (Button OnClick)
    public void OnNextLevelButton()  => GameManager.Instance.LoadNextLevel();
    public void OnRestartButton()
    {
        GameManager.Instance.FullReset();
        GameManager.Instance.RestartLevel();
    }
    public void OnMainMenuButton()   => GameManager.Instance.GoToMainMenu();
}
