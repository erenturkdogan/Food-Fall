using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public GameObject pausePanel;
    private bool _paused = false;

    void Awake()
    {
        // Sadece kendi script'i kontrol et, gameObject'i destroy etme
        // (aynı objedeki diğer scriptleri öldürmemek için)
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    public void TogglePause()
    {
        _paused = !_paused;
        Time.timeScale = _paused ? 0f : 1f;
        if (pausePanel != null)
            pausePanel.SetActive(_paused);

        if (AudioManager.Instance != null)
        {
            if (_paused) AudioManager.Instance.PauseMusic();
            else AudioManager.Instance.ResumeMusic();
        }
    }

    public void Resume()
    {
        _paused = false;
        Time.timeScale = 1f;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeMusic();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameManager.Instance.score = 0; // mevcut level'da kal
        GameManager.Instance.RestartLevel();
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
        if (Instance == this) Instance = null;
    }
}
