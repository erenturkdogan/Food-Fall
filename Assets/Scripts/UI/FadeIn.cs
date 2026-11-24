using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Açılışta siyah perde 1→0 alpha ile şeffaflaşır.
/// Image siyah, alpha 1 ile başlamalı.
/// </summary>
[RequireComponent(typeof(Image))]
public class FadeIn : MonoBehaviour
{
    public float duration = 0.8f;
    public float delay = 0.1f;

    private Image _img;
    private float _t;

    void Start()
    {
        _img = GetComponent<Image>();
        SetAlpha(1f);
        _img.raycastTarget = false;
        _t = -delay;
    }

    void Update()
    {
        _t += Time.unscaledDeltaTime;
        if (_t < 0f) return;
        float a = 1f - Mathf.Clamp01(_t / duration);
        SetAlpha(a);
        if (a <= 0f) { gameObject.SetActive(false); enabled = false; }
    }

    void SetAlpha(float a)
    {
        var c = _img.color; c.a = a; _img.color = c;
    }
}
