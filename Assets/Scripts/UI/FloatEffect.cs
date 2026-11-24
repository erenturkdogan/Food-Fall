using UnityEngine;

/// <summary>
/// Objeyi yukarı-aşağı sallar (havada süzülme efekti).
/// </summary>
public class FloatEffect : MonoBehaviour
{
    public float amplitude = 20f;
    public float speed = 1.5f;

    private Vector3 _startPos;

    void Start() { _startPos = transform.localPosition; }

    void Update()
    {
        float offset = Mathf.Sin(Time.unscaledTime * speed) * amplitude;
        transform.localPosition = _startPos + Vector3.up * offset;
    }
}
