using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Main Menu arka planında dekoratif olarak rastgele yiyecek görselleri düşürür.
/// Canvas'ın altında bir RectTransform container'a Image'lar instantiate eder.
/// </summary>
public class BackgroundFoodRain : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform container;        // arka plan paneli
    public Sprite[] foodSprites;           // düşecek görseller
    public int spritePool = 8;             // ekranda en fazla kaç tane
    public float minSpeed = 60f;
    public float maxSpeed = 160f;
    public float minScale = 0.6f;
    public float maxScale = 1.2f;
    public float spawnInterval = 0.6f;
    public Color iconColor = new Color(1f, 1f, 1f, 0.4f); // hafif şeffaf

    void Start()
    {
        if (container == null) container = (RectTransform)transform;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOne()
    {
        if (foodSprites == null || foodSprites.Length == 0) return;

        // Container'ın aktif sprite sayısı
        if (container.childCount >= spritePool) return;

        GameObject go = new GameObject("FoodDeco", typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(container, false);

        var img = go.AddComponent<Image>();
        img.sprite = foodSprites[Random.Range(0, foodSprites.Length)];
        img.color = iconColor;
        img.preserveAspect = true;
        img.raycastTarget = false;

        float scale = Random.Range(minScale, maxScale);
        rt.sizeDelta = new Vector2(100 * scale, 100 * scale);

        float halfW = container.rect.width * 0.5f;
        float startY = container.rect.height * 0.5f + 60f;
        float endY = -container.rect.height * 0.5f - 60f;
        rt.anchoredPosition = new Vector2(Random.Range(-halfW, halfW), startY);

        float speed = Random.Range(minSpeed, maxSpeed);
        float rotSpeed = Random.Range(-90f, 90f);

        StartCoroutine(MoveAndDestroy(rt, endY, speed, rotSpeed));
    }

    IEnumerator MoveAndDestroy(RectTransform rt, float endY, float speed, float rotSpeed)
    {
        while (rt != null && rt.anchoredPosition.y > endY)
        {
            rt.anchoredPosition += Vector2.down * speed * Time.unscaledDeltaTime;
            rt.Rotate(0, 0, rotSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        if (rt != null) Destroy(rt.gameObject);
    }
}
