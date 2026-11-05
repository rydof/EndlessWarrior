using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject slimePrefab, beePrefab, orcPrefab, wolfPrefab;

    [Header("Wave Settings")]
    public int initialSlimeCount = 2;
    public float slimeBaseGrowth = 1.1f, beeBaseGrowth = 1.2f, orcBaseGrowth = 1.3f, wolfBaseGrowth = 1.4f;
    public float spawnRadius = 10f;
    public float safeZoneRadius = 3f;
    public Transform playerTransform;

    [Header("Playable Area")]
    public Collider2D playableAreaCollider;

    [Header("Dependencies")]
    public PlayerCoins playerCoins;
    public UpgradeStatsUI upgradeStatsUI;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    [Header("Coin Collection Phase")]
    public float coinCollectionDuration = 1f;
    public float uiDelayAfterCoinCollection = 1.5f;

    [Header("Enemy Upgrade Thresholds")]
    public int slimeUpgradeThreshold = 10;
    public int beeUpgradeThreshold = 15;
    public int wolfUpgradeThreshold = 20;
    public int orcUpgradeThreshold = 25;
    public float upgradeInterval = 5f;
    public float enemyCountReductionFactor = 0.7f;

    private int currentWave = 0;
    private List<GameObject> currentEnemies = new();
    private bool isWaveInProgress = false;
    private bool isPlayerDead = false;

    public delegate void WaveChanged(int currentWave);
    public event WaveChanged OnWaveChanged;
    public event Action<int> OnPlayerDeath;

    public int Wave
    {
        get => currentWave;
        set
        {
            currentWave = value;
            OnWaveChanged?.Invoke(currentWave);
        }
    }

    private void Start()
    {
        if (playerTransform == null) Debug.LogError("PlayerTransform not assigned!");
        if (playerCoins == null) Debug.LogError("PlayerCoins not assigned!");
        if (gameOverUI != null) gameOverUI.SetActive(false);
        else Debug.LogError("GameOverUI not assigned!");

        OnWaveChanged?.Invoke(currentWave);
        StartNextWave();
    }

    public void HandlePlayerDeath()
    {
        if (isPlayerDead) return;

        isPlayerDead = true;
        isWaveInProgress = false;

        foreach (var enemy in currentEnemies.ToArray())
        {
            if (enemy != null) Destroy(enemy);
        }
        currentEnemies.Clear();

        if (gameOverUI != null) gameOverUI.SetActive(true);
        else Debug.LogError("GameOverUI not assigned!");

        OnPlayerDeath?.Invoke(currentWave);
    }

    public void BeginNextWave() => StartNextWave();

    private void StartNextWave()
    {
        if (isPlayerDead) return;

        isWaveInProgress = true;
        currentWave++;
        OnWaveChanged?.Invoke(currentWave);
        SpawnEnemiesForWave(currentWave);
    }

    private void SpawnEnemiesForWave(int wave)
    {
        int slimeCount = Mathf.Max(initialSlimeCount, Mathf.FloorToInt(Mathf.Pow(wave, slimeBaseGrowth)));
        int beeCount = wave >= 5 ? Mathf.FloorToInt(Mathf.Pow(wave - 4, beeBaseGrowth)) : 0;
        int orcCount = wave >= 10 ? Mathf.FloorToInt(Mathf.Pow(wave - 9, orcBaseGrowth)) : 0;
        int wolfCount = wave >= 15 ? Mathf.FloorToInt(Mathf.Pow(wave - 14, wolfBaseGrowth)) : 0;

        if (wave >= slimeUpgradeThreshold)
        {
            slimeCount = Mathf.FloorToInt(slimeCount * enemyCountReductionFactor);
            beeCount = Mathf.FloorToInt(beeCount * enemyCountReductionFactor);
            orcCount = Mathf.FloorToInt(orcCount * enemyCountReductionFactor);
            wolfCount = Mathf.FloorToInt(wolfCount * enemyCountReductionFactor);
        }

        SpawnEnemies(slimePrefab, slimeCount);
        if (wave >= 5) SpawnEnemies(beePrefab, beeCount);
        if (wave >= 10) SpawnEnemies(orcPrefab, orcCount);
        if (wave >= 15) SpawnEnemies(wolfPrefab, wolfCount);
    }

    private void SpawnEnemies(GameObject enemyPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPosition = Vector2.zero;
            int attempts = 0;

            do
            {
                if (playableAreaCollider != null)
                {
                    var bounds = playableAreaCollider.bounds;
                    float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                    float y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
                    spawnPosition = new Vector2(x, y);
                }
                else
                {
                    spawnPosition = playerTransform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * spawnRadius);
                }
                attempts++;
            }
            while ((Vector2.Distance(spawnPosition, playerTransform.position) < safeZoneRadius ||
                    (playableAreaCollider != null && !playableAreaCollider.OverlapPoint(spawnPosition))) && attempts < 100);

            if (attempts >= 100)
            {
                Debug.LogWarning("Failed to find valid spawn position");
                continue;
            }

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemies.Add(enemy);

            int upgradeLevel = GetUpgradeLevel(enemyPrefab);
            if (upgradeLevel > 0)
            {
                enemy.GetComponent<Enemy_Health>()?.ApplyUpgrades(upgradeLevel);
                enemy.GetComponent<Enemy_Combat>()?.ApplyUpgrades(upgradeLevel);
            }
        }
    }

    private int GetUpgradeLevel(GameObject prefab)
    {
        if (prefab == slimePrefab && currentWave >= slimeUpgradeThreshold)
            return Mathf.FloorToInt((currentWave - slimeUpgradeThreshold) / upgradeInterval) + 1;
        if (prefab == beePrefab && currentWave >= beeUpgradeThreshold)
            return Mathf.FloorToInt((currentWave - beeUpgradeThreshold) / upgradeInterval) + 1;
        if (prefab == orcPrefab && currentWave >= orcUpgradeThreshold)
            return Mathf.FloorToInt((currentWave - orcUpgradeThreshold) / upgradeInterval) + 1;
        if (prefab == wolfPrefab && currentWave >= wolfUpgradeThreshold)
            return Mathf.FloorToInt((currentWave - wolfUpgradeThreshold) / upgradeInterval) + 1;
        return 0;
    }

    private void Update()
    {
        currentEnemies.RemoveAll(e => e == null);

        if (isWaveInProgress && currentEnemies.Count == 0 && !isPlayerDead)
        {
            isWaveInProgress = false;
            StartCoroutine(HandleWaveEnd());
        }
    }

    private IEnumerator HandleWaveEnd()
    {
        if (isPlayerDead) yield break;

        yield return StartCoroutine(CoinCollectionPhase());

        if (!isPlayerDead)
            upgradeStatsUI?.OpenUpgradeUI();
    }

    private IEnumerator CoinCollectionPhase()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        float maxDistance = 0f;
        foreach (var coin in coins)
        {
            float d = Vector2.Distance(playerTransform.position, coin.transform.position);
            if (d > maxDistance) maxDistance = d;
        }

        float totalDuration = Mathf.Max(coinCollectionDuration * maxDistance, coinCollectionDuration);
        WaveUIManager ui = UnityEngine.Object.FindAnyObjectByType<WaveUIManager>();

        float timer = 3 + totalDuration;
        while (timer > 0)
        {
            if (GameObject.FindGameObjectsWithTag("Coin").Length == 0)
            {
                timer = 0;
                ui?.UpdateTimerText(timer);
                yield return new WaitForSeconds(uiDelayAfterCoinCollection);
                break;
            }

            timer -= Time.deltaTime;
            ui?.UpdateTimerText(timer);
            yield return null;
        }

        ui?.ResetWaveText(currentWave);
        DespawnAllCoins();
    }

    private void DespawnAllCoins()
    {
        foreach (var coin in GameObject.FindGameObjectsWithTag("Coin"))
        {
            Destroy(coin);
        }
    }
}
