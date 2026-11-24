using UnityEngine;

/// <summary>
/// Objeyi sürekli büyütüp küçültür. Title text veya butonlara ekle.
/// </summary>
public class PulseEffect : MonoBehaviour
{
    public float minScale = 1.0f;
    public float maxScale = 1.1f;
    public float speed = 2f;

    private Vector3 _baseScale;

    void Start() { _baseScale = transform.localScale; }

    void Update()
    {
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f; // 0..1
        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = _baseScale * s;
    }
}
