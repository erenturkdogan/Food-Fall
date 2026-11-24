using UnityEngine;

public class TimerController : MonoBehaviour
{
    private float _timeRemaining;
    private bool _running = false;

    public event System.Action<float> OnTimerTick;   // kalan süre
    public event System.Action OnTimerExpired;

    void Start()
    {
        // Endless modda süre yok — timer kapalı
        if (GameManager.Instance.CurrentLevel.isEndless)
        {
            _running = false;
            OnTimerTick?.Invoke(0f); // UI'da 0/sonsuz gösterilebilir
            return;
        }

        _timeRemaining = GameManager.Instance.CurrentLevel.timeLimitSeconds;
        _running = true;

        RecipeManager.Instance.OnRecipeComplete += StopTimer;
        GameManager.Instance.OnGameOver += StopTimer;
    }

    void OnDestroy()
    {
        if (RecipeManager.Instance != null)
            RecipeManager.Instance.OnRecipeComplete -= StopTimer;
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= StopTimer;
    }

    void Update()
    {
        if (!_running) return;

        _timeRemaining -= Time.deltaTime;
        OnTimerTick?.Invoke(_timeRemaining);

        if (_timeRemaining <= 0f)
        {
            _running = false;
            OnTimerExpired?.Invoke();
            GameManager.Instance.TriggerGameOver();
        }
    }

    private void StopTimer()
    {
        _running = false;
    }
}
