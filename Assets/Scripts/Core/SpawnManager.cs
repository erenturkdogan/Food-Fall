using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject ingredientPrefab;

    [Header("Spawn Y — kamera üstünden biraz yukarı")]
    [Tooltip("Viewport Y=1 (üst kenar) + bu kadar dünya birimi yukarısından spawn edilir")]
    public float spawnYOffset = 1f;

    [Tooltip("Ekranın kaç birim iç kısmından spawn edilsin (kenardan boşluk)")]
    public float spawnXMargin = 0.4f;

    [Tooltip("Oyun başlangıcında ilk spawn'a kadar bekleme süresi (sn). Countdown ile uyumlu olsun.")]
    public float initialDelay = 4f;

    [Header("Spawn Yoğunluğu")]
    [Tooltip("Tarif malzemelerinin havuzdaki ağırlığı (yüksek = daha sık düşer)")]
    public int recipeWeight = 6;
    [Tooltip("Decoy malzemelerinin havuzdaki ağırlığı")]
    public int decoyWeight = 1;
    [Tooltip("Spawn aralığına eklenecek rastgelelik (± bu kadar saniye)")]
    public float intervalRandomness = 0.4f;

    private LevelData _level;
    private Camera _cam;
    private float _spawnY;
    private float _spawnXMin;
    private float _spawnXMax;
    private float _destroyY;

    private List<IngredientData> _weightedPool = new();
    private Coroutine _spawnRoutine;

    private float _curFallSpeed;
    private float _curSpawnInterval;

    void Start()
    {
        _cam = Camera.main;
        _level = GameManager.Instance.CurrentLevel;
        _curFallSpeed = _level.fallSpeed;
        _curSpawnInterval = _level.spawnInterval;

        CalculateBounds();
        BuildWeightedPool();

        _spawnRoutine = StartCoroutine(SpawnLoop());

        if (_level.isEndless)
        {
            // Endless: her yeni tarifte havuzu yenile + zorluğu artır
            RecipeManager.Instance.OnRecipeRegenerated += OnEndlessRound;
        }
        else
        {
            RecipeManager.Instance.OnRecipeComplete += StopSpawning;
        }
        GameManager.Instance.OnGameOver += StopSpawning;
    }

    void OnDestroy()
    {
        if (RecipeManager.Instance != null)
        {
            RecipeManager.Instance.OnRecipeComplete -= StopSpawning;
            RecipeManager.Instance.OnRecipeRegenerated -= OnEndlessRound;
        }
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= StopSpawning;
    }

    private void OnEndlessRound()
    {
        // Zorluğu artır
        _curFallSpeed += _level.endlessSpeedStep;
        _curSpawnInterval = Mathf.Max(0.4f, _curSpawnInterval - _level.endlessIntervalStep);
        BuildWeightedPool();
    }

    private void CalculateBounds()
    {
        float camHalfH = _cam.orthographicSize;
        float camHalfW = camHalfH * _cam.aspect;
        Vector3 camPos = _cam.transform.position;

        _spawnY  = camPos.y + camHalfH + spawnYOffset;
        _destroyY = camPos.y - camHalfH - 1f;
        _spawnXMin = camPos.x - camHalfW + spawnXMargin;
        _spawnXMax = camPos.x + camHalfW - spawnXMargin;
    }

    private void BuildWeightedPool()
    {
        _weightedPool.Clear();

        // Tarif malzemeleri yüksek ağırlık — aktif tarif state'inden oku
        foreach (var kv in RecipeManager.Instance.GetState())
            for (int i = 0; i < recipeWeight * kv.Value.required; i++)
                _weightedPool.Add(kv.Key);

        // Decoy malzemeler düşük ağırlık
        var decoys = _level.isEndless ? _level.endlessPool : _level.possibleIngredients;
        if (decoys != null)
            foreach (var ing in decoys)
                if (ing != null)
                    for (int i = 0; i < decoyWeight; i++)
                        _weightedPool.Add(ing);
    }

    private IEnumerator SpawnLoop()
    {
        // İlk spawndan önce bekleme (countdown ile uyum için)
        yield return new WaitForSecondsRealtime(initialDelay);

        // Tarif kesin hazır olduktan sonra havuzu yeniden kur
        BuildWeightedPool();

        while (true)
        {
            SpawnOne();
            float wait = _curSpawnInterval + Random.Range(-intervalRandomness, intervalRandomness);
            wait = Mathf.Max(0.15f, wait);
            yield return new WaitForSeconds(wait);
        }
    }

    private void SpawnOne()
    {
        if (_weightedPool.Count == 0) return;

        float x = Random.Range(_spawnXMin, _spawnXMax);
        Vector3 pos = new Vector3(x, _spawnY, 0f);

        GameObject obj = Instantiate(ingredientPrefab, pos, Quaternion.identity);
        IngredientData data = _weightedPool[Random.Range(0, _weightedPool.Count)];
        obj.GetComponent<FallingIngredient>().Init(data, _curFallSpeed, _destroyY);
    }

    private void StopSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }
}
