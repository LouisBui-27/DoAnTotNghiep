using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<EnemyWave> waves;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private float spawnDistance = 7f;
    [SerializeField] private float initialSpawnDelay = 1f;
   // [SerializeField] private GameObject winPanel; // Kéo UI Win Panel từ Inspector
    [SerializeField] private int levelIndex = 1;  // Ví dụ Level1 = 1, Level2 = 2...

    private int aliveEnemyCount = 0;
    private int deadEnemyCount = 0;
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Text enemyDieTxt; 
    [SerializeField] private Text enemyAliveTxt; 


    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    public int CurrentWaveNumber => currentWaveIndex + 1;
    public int TotalWaves => waves.Count;
    public bool IsFinalWaveComplete => currentWaveIndex >= waves.Count;

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        currentWaveIndex = 0;
        aliveEnemyCount = 0;
        isSpawning = false;
        deadEnemyCount = 0;
        UpdateEnemyUI();
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    public void StartSpawning()
    {
        if (isSpawning || player == null) return;

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnWaveSequence());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    private IEnumerator SpawnWaveSequence()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while (currentWaveIndex < waves.Count)
        {
            EnemyWave currentWave = waves[currentWaveIndex];
            yield return StartCoroutine(SpawnWave(currentWave));

            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                float waitTime = currentWave.isBossWave ? timeBetweenWaves * 2f : timeBetweenWaves;
                yield return new WaitForSeconds(waitTime);

                // Có thể thêm event thông báo giữa các wave
                // WaveBreakEvent?.Invoke(waitTime);
            }
        }

        isSpawning = false;
        // WaveCompletionEvent?.Invoke();
    }

    private IEnumerator SpawnWave(EnemyWave wave)
    {
        yield return new WaitForSeconds(wave.waveSpawnDelay);

        foreach (var entry in wave.enemiesInWave)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemyPrefab, wave.spawnPattern, entry.expPrefabs, entry.baseHp, entry.hpScaleFactorPerWave);

                float delay = wave.spawnRateBetweenEnemies * entry.individualSpawnRateMultiplier;
                yield return new WaitForSeconds(delay);
            }
        }
    }

    private void SpawnEnemy(GameObject prefab, SpawnPattern pattern, GameObject expPrefab, float baseHp, float hpScaleFactorPerWave)
    {
        if (player == null || prefab == null) return;

        Vector2 spawnPosition = GetSpawnPosition(pattern);
        GameObject enemy = ObjectPooling.Instance.GetFromPool(prefab, spawnPosition, Quaternion.identity);

        if (enemy == null) return;

        aliveEnemyCount++;

        if (enemy.TryGetComponent<EnemyController>(out var enemyController))
        {
            enemyController.SetTarget(player);
            enemyController.Initialize(currentWaveIndex, baseHp, hpScaleFactorPerWave);

            if (!(enemyController is BossController) && expPrefab != null)
            {
                enemyController.SetExpPrefab(expPrefab);
            }

            enemyController.OnDeath += () => HandleEnemyDeath();
        }
        else if (enemy.TryGetComponent<IBoss>(out var bossController))
        {

            bossController.OnDeath += () => HandleEnemyDeath();
        }
        UpdateEnemyUI();
    }

    private void HandleEnemyDeath()
    {
        aliveEnemyCount--;
        deadEnemyCount++;
        UpdateEnemyUI();
        // Kiểm tra điều kiện thắng khi không còn enemy/boss nào sống sót
        if (IsFinalWaveComplete && aliveEnemyCount <= 0)
        {
            HandleWinGame();
        }
    }
    private void HandleWinGame()
    {
        MissionManager.Instance.OnLevelCompleted(levelIndex);
       // LevelUnlock.MarkLevelCompleted(levelIndex); // <-- THAY ĐỔI TẠI ĐÂY
        GameOverUI.instance.ShowResult(true);

        
    }
    private Vector2 GetSpawnPosition(SpawnPattern pattern)
    {
        if (player == null) return Vector2.zero;

        switch (pattern)
        {
            case SpawnPattern.Rectangle:
                {
                    int side = Random.Range(0, 4); // 0=top, 1=bottom, 2=left, 3=right
                    float x = 0, y = 0;

                    switch (side)
                    {
                        case 0: // top
                            x = Random.Range(-spawnDistance, spawnDistance);
                            y = spawnDistance;
                            break;
                        case 1: // bottom
                            x = Random.Range(-spawnDistance, spawnDistance);
                            y = -spawnDistance;
                            break;
                        case 2: // left
                            x = -spawnDistance;
                            y = Random.Range(-spawnDistance, spawnDistance);
                            break;
                        case 3: // right
                            x = spawnDistance;
                            y = Random.Range(-spawnDistance, spawnDistance);
                            break;
                    }

                    return (Vector2)player.position + new Vector2(x, y);
                }

            case SpawnPattern.Circle:
            default:
                float angle = Random.Range(0f, Mathf.PI * 2f); // Góc từ 0 đến 2π
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                return (Vector2)player.position + direction * spawnDistance;
        }
    }
    private void UpdateEnemyUI()
    {
        if (enemyAliveTxt != null)
            enemyAliveTxt.text = $"{aliveEnemyCount}";

        if (enemyDieTxt != null)
            enemyDieTxt.text = $"{deadEnemyCount}";
    }
}
