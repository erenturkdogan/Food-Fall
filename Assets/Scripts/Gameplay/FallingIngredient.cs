using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class FallingIngredient : MonoBehaviour
{
    public IngredientData Data { get; private set; }

    private float _fallSpeed;
    private float _destroyY;
    private bool _caught = false;
    public bool IsCaught => _caught;

    [Header("Dönme")]
    [Tooltip("Saniyedeki maksimum dönüş hızı (derece). Her malzemeye rastgele atanır.")]
    public float maxRotateSpeed = 90f;
    private float _rotateSpeed;

    [Header("Salınım (opsiyonel)")]
    [Tooltip("Düşerken hafif sağa-sola süzülme genliği (0 = kapalı)")]
    public float swayAmplitude = 0.3f;
    public float swaySpeed = 2f;
    private float _swayPhase;
    private float _baseX;

    public void Init(IngredientData data, float fallSpeed, float destroyY)
    {
        Data = data;
        _fallSpeed = fallSpeed;
        _destroyY = destroyY;
        if (data == null) { Debug.LogError("FallingIngredient: data null!"); Destroy(gameObject); return; }
        if (data.sprite == null)
            Debug.LogWarning($"IngredientData '{data.name}' için sprite atanmamış!");
        else
            GetComponent<SpriteRenderer>().sprite = data.sprite;

        // Her malzeme rastgele yönde ve hızda döner
        _rotateSpeed = Random.Range(-maxRotateSpeed, maxRotateSpeed);
        _swayPhase = Random.Range(0f, Mathf.PI * 2f);
        _baseX = transform.position.x;
    }

    void Update()
    {
        if (_caught) return;

        // Aşağı düş
        float newY = transform.position.y - _fallSpeed * Time.deltaTime;

        // Hafif sağa-sola süzülme
        float x = transform.position.x;
        if (swayAmplitude > 0f)
        {
            _swayPhase += swaySpeed * Time.deltaTime;
            x = _baseX + Mathf.Sin(_swayPhase) * swayAmplitude;
        }

        transform.position = new Vector3(x, newY, 0f);

        // Dönme
        transform.Rotate(0f, 0f, _rotateSpeed * Time.deltaTime);

        if (transform.position.y < _destroyY)
            Destroy(gameObject);
    }

    public void Catch()
    {
        _caught = true;
        Destroy(gameObject);
    }
}
