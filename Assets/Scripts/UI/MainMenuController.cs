using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public SceneFader fader; // Inspector'dan ata

    [Tooltip("Endless level'ın GameManager.levels dizisindeki index'i (genelde 3 = 4. level)")]
    public int endlessLevelIndex = 3;

    public void StartGame()
    {
        GameManager.StartLevelIndex = 0; // GameManager henüz yoksa Awake'te uygulanır
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FullReset();
            GameManager.Instance.currentLevelIndex = 0; // zaten varsa direkt set et
        }
        LoadGame();
    }

    public void StartEndless()
    {
        GameManager.StartLevelIndex = endlessLevelIndex;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FullReset();
            GameManager.Instance.currentLevelIndex = endlessLevelIndex;
        }
        LoadGame();
    }

    private void LoadGame()
    {
        if (fader != null)
            fader.FadeAndLoad("GameScene");
        else
            SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
