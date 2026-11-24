using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Level başında 3-2-1-GO geri sayım gösterir.
/// O sırada oyun durur (Time.timeScale = 0).
/// </summary>
public class LevelStartCountdown : MonoBehaviour
{
    [Header("UI")]
    public GameObject countdownPanel;     // tam ekran panel (yarı şeffaf perde)
    public TextMeshProUGUI countdownText; // büyük rakam

    [Header("Timing")]
    public float startDelay = 0.5f;       // fade-in için kısa bekleme
    public int startNumber = 3;           // 3'ten say
    public float numberDuration = 1f;     // her rakamın süresi
    public string finalWord = "GO!";

    void Awake()
    {
        // Diğer scriptlerin Start'ından önce timeScale'i 0 yap
        Time.timeScale = 0f;
    }

    void Start()
    {
        if (countdownPanel != null) countdownPanel.SetActive(true);
        StartCoroutine(RunCountdown());
    }

    private IEnumerator RunCountdown()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        for (int i = startNumber; i >= 1; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                StartCoroutine(PopAnimation(countdownText.transform));
            }
            yield return new WaitForSecondsRealtime(numberDuration);
        }

        if (countdownText != null)
        {
            countdownText.text = finalWord;
            StartCoroutine(PopAnimation(countdownText.transform));
        }
        yield return new WaitForSecondsRealtime(numberDuration * 0.7f);

        Time.timeScale = 1f;
        if (countdownPanel != null) countdownPanel.SetActive(false);
    }

    private IEnumerator PopAnimation(Transform t)
    {
        Vector3 baseScale = Vector3.one;
        float dur = 0.3f;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = elapsed / dur;
            float s = Mathf.Lerp(1.6f, 1f, k);
            t.localScale = baseScale * s;
            yield return null;
        }
        t.localScale = baseScale;
    }
}
