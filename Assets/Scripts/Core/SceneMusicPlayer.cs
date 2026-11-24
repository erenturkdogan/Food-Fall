using UnityEngine;

/// <summary>
/// Her sahneye eklenir. Sahne açıldığında uygun müziği başlatır.
/// </summary>
public class SceneMusicPlayer : MonoBehaviour
{
    public enum MusicType { Menu, Game, None }
    public MusicType musicToPlay = MusicType.Game;

    void Start()
    {
        if (AudioManager.Instance == null) return;

        switch (musicToPlay)
        {
            case MusicType.Menu: AudioManager.Instance.PlayMenuMusic(); break;
            case MusicType.Game: AudioManager.Instance.PlayGameMusic(); break;
            case MusicType.None: AudioManager.Instance.StopMusic(); break;
        }
    }
}
