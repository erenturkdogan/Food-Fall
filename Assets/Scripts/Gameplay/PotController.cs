using UnityEngine;

/// <summary>
/// Tencereyi touch/mouse ile yatay hareket ettirir.
/// Sınırlar kamera viewport'undan otomatik hesaplanır.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PotController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 12f;
    [Tooltip("Tencerenin yarı genişliği — sol/sağ sınır kaydırması için")]
    public float potHalfWidth = 0.5f;

    [Header("Scoring")]
    public int correctCatchPoints = 100;
    public float wrongCatchQualityPenalty = 15f;
    public float correctCatchQualityBonus = 5f;

    private float _leftBound;
    private float _rightBound;
    private float _targetX;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        CalculateBounds();
        _targetX = transform.position.x;
    }

    private void CalculateBounds()
    {
        // Kameranın dünya koordinatlarındaki sol/sağ kenarı
        float camHalfWidth = _cam.orthographicSize * _cam.aspect;
        _leftBound  = _cam.transform.position.x - camHalfWidth + potHalfWidth;
        _rightBound = _cam.transform.position.x + camHalfWidth - potHalfWidth;
    }

    void Update()
    {
        ReadInput();
        MovePot();
    }

    private void ReadInput()
    {
#if ENABLE_INPUT_SYSTEM
        var touch = UnityEngine.InputSystem.Touchscreen.current;
        if (touch != null && touch.primaryTouch.press.isPressed)
        {
            Vector2 sp = touch.primaryTouch.position.ReadValue();
            Vector3 world = _cam.ScreenToWorldPoint(new Vector3(sp.x, sp.y, 0f));
            _targetX = Mathf.Clamp(world.x, _leftBound, _rightBound);
            return;
        }
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null && mouse.leftButton.isPressed)
        {
            Vector2 sp = mouse.position.ReadValue();
            Vector3 world = _cam.ScreenToWorldPoint(new Vector3(sp.x, sp.y, 0f));
            _targetX = Mathf.Clamp(world.x, _leftBound, _rightBound);
        }
#else
        if (Input.touchCount > 0)
        {
            Vector3 world = _cam.ScreenToWorldPoint(Input.GetTouch(0).position);
            _targetX = Mathf.Clamp(world.x, _leftBound, _rightBound);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 world = _cam.ScreenToWorldPoint(Input.mousePosition);
            _targetX = Mathf.Clamp(world.x, _leftBound, _rightBound);
        }
#endif
    }

    private void MovePot()
    {
        float newX = Mathf.MoveTowards(transform.position.x, _targetX, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<FallingIngredient>(out var ingredient)) return;
        if (ingredient.IsCaught) return;

        Debug.Log($"Pot yakaladı: {ingredient.Data?.name}. RecipeManager null mu? {RecipeManager.Instance == null}");
        if (RecipeManager.Instance == null) return;
        bool correct = RecipeManager.Instance.CatchIngredient(ingredient.Data);
        ingredient.Catch();

        if (correct)
        {
            GameManager.Instance.IncreaseCombo();
            GameManager.Instance.AddScore(correctCatchPoints);
            GameManager.Instance.IncreaseQuality(correctCatchQualityBonus);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCatchCorrect();
        }
        else
        {
            GameManager.Instance.ResetCombo();
            GameManager.Instance.ReduceQuality(wrongCatchQualityPenalty);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCatchWrong();
            if (ScreenFeedback.Instance != null) ScreenFeedback.Instance.PlayWrongFeedback();
        }
    }
}
