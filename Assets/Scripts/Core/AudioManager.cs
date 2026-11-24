using UnityEngine;

/// <summary>
/// Merkezi ses yöneticisi. Müzik (loop) ve efektleri (one-shot) çalar.
/// DontDestroyOnLoad ile sahneler arası kalır.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;  // loop müzik
    public AudioSource sfxSource;    // efektler

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;      // background

    [Header("SFX Clips")]
    public AudioClip catchCorrect;   // doğru yakalama
    public AudioClip catchWrong;     // yanlış yakalama
    public AudioClip buttonClick;    // buton tıklama

    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 0.189f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float buttonClickVolume = 0.3f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // --- Müzik ---
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayGameMusic() => PlayMusic(gameMusic);
    public void StopMusic() { if (musicSource != null) musicSource.Stop(); }
    public void PauseMusic() { if (musicSource != null) musicSource.Pause(); }
    public void ResumeMusic() { if (musicSource != null) musicSource.UnPause(); }

    // --- Efektler ---
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayCatchCorrect() => PlaySFX(catchCorrect);
    public void PlayCatchWrong()   => PlaySFX(catchWrong);
    public void PlayButtonClick()
    {
        if (sfxSource == null || buttonClick == null) return;
        sfxSource.PlayOneShot(buttonClick, buttonClickVolume);
    }
}
