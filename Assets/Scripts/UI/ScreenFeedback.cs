using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Yanlış malzeme alınca ekran kırmızı flash + kamera sallanma efekti.
/// Singleton — PotController buradan tetikler.
/// </summary>
public class ScreenFeedback : MonoBehaviour
{
    public static ScreenFeedback Instance { get; private set; }

    [Header("Red Flash")]
    public Image flashImage;            // tam ekran kırmızı Image (alpha 0 başlar)
    public float flashAlpha = 0.35f;
    public float flashDuration = 0.3f;

    [Header("Shake")]
    public Transform shakeTarget;       // sallanacak obje (Camera veya Canvas)
    public float shakeDuration = 0.25f;
    public float shakeMagnitude = 0.2f;

    private Vector3 _shakeBasePos;
    private Coroutine _flashRoutine;
    private Coroutine _shakeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void Start()
    {
        if (shakeTarget != null) _shakeBasePos = shakeTarget.localPosition;
        if (flashImage != null) SetFlashAlpha(0f);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PlayWrongFeedback()
    {
        if (flashImage != null)
        {
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }
        if (shakeTarget != null)
        {
            if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
            _shakeRoutine = StartCoroutine(ShakeRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - (t / flashDuration); // 1 → 0
            SetFlashAlpha(flashAlpha * k);
            yield return null;
        }
        SetFlashAlpha(0f);
    }

    private IEnumerator ShakeRoutine()
    {
        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;
            float damp = 1f - (t / shakeDuration);
            float x = Random.Range(-1f, 1f) * shakeMagnitude * damp;
            float y = Random.Range(-1f, 1f) * shakeMagnitude * damp;
            shakeTarget.localPosition = _shakeBasePos + new Vector3(x, y, 0f);
            yield return null;
        }
        shakeTarget.localPosition = _shakeBasePos;
    }

    private void SetFlashAlpha(float a)
    {
        var c = flashImage.color;
        c.a = a;
        flashImage.color = c;
    }
}
