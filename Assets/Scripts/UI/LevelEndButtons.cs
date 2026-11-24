using UnityEngine;

/// <summary>
/// Level Complete ve Game Over panel butonları için.
/// Singleton üzerinden çağrılır, sahne reload edince referans bozulmaz.
/// </summary>
public class LevelEndButtons : MonoBehaviour
{
    [Tooltip("Sahne geçişi için fade perdesi (opsiyonel)")]
    public SceneFader fader;

    public void OnNextLevel()
    {
        var gm = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();
        if (gm == null) { Debug.LogError("GameManager bulunamadı!"); return; }

        // Index'i ilerlet ve state'i sıfırla
        gm.currentLevelIndex++;
        gm.ResetRuntimeState();
        if (gm.currentLevelIndex >= gm.levels.Length)
        {
            LoadScene("WinScene");
            return;
        }
        LoadScene("GameScene");
    }

    public void OnRestart()
    {
        var gm = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();
        if (gm == null) return;
        // Mevcut level'da kal (endless ise endless'a, level 2 ise level 2'ye), skoru sıfırla
        gm.score = 0;
        gm.ResetRuntimeState();
        LoadScene("GameScene");
    }

    public void OnMainMenu()
    {
        var gm = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();
        if (gm == null) return;
        gm.FullReset();
        LoadScene("MainMenu");
    }

    private void LoadScene(string sceneName)
    {
        // Fade varsa onunla, yoksa direkt yükle
        Time.timeScale = 1f;
        if (fader != null)
            fader.FadeAndLoad(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
