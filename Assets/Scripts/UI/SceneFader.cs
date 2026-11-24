using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Sahne geçişinde fade-out perdesi.
/// Canvas'a Image (siyah, tüm ekran) koy, alpha 0'la başlat, bu scripti ekle.
/// </summary>
[RequireComponent(typeof(Image))]
public class SceneFader : MonoBehaviour
{
    public float fadeDuration = 0.8f;
    public float delayBeforeLoad = 0.2f;

    private Image _img;

    void Awake()
    {
        _img = GetComponent<Image>();
        SetAlpha(0f);
        _img.raycastTarget = false; // başlangıçta tıklamaları engelleme
    }

    public void FadeAndLoad(string sceneName)
    {
        StartCoroutine(DoFadeOut(sceneName));
    }

    private IEnumerator DoFadeOut(string sceneName)
    {
        _img.raycastTarget = true; // tıklamaları kapat
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
        SetAlpha(1f);
        yield return new WaitForSecondsRealtime(delayBeforeLoad);
        SceneManager.LoadScene(sceneName);
    }

    private void SetAlpha(float a)
    {
        var c = _img.color;
        c.a = a;
        _img.color = c;
    }
}
