using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class EnemyWave
{
    public List<EnemySpawnEntry> enemiesInWave; // Danh sách các loại quái và số lượng/tỷ lệ của chúng

    public float waveSpawnDelay = 1f; // Thời gian chờ trước khi bắt đầu spawn wave này
    public float spawnRateBetweenEnemies = 0.5f; // Tốc độ spawn chung cho các enemy trong wave này
    public bool isBossWave = false;
    public SpawnPattern spawnPattern = SpawnPattern.Circle;
}

[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab; // Prefab của loại quái này
    public int count; // Số lượng quái loại này sẽ spawn
    public float individualSpawnRateMultiplier = 1f; // Tốc độ spawn riêng cho loại quái này (nhân với waveSpawnRate)
    public GameObject expPrefabs; // Lượng XP riêng cho loại quái này
    public float baseHp = 10f; // Máu gốc của loại quái này
    public float hpScaleFactorPerWave = 0f; // Hệ số tăng máu theo wave (0 = không tăng, 1 = tăng theo waveIndex)
    // Ví dụ: nếu baseHp=10, hpScaleFactorPerWave=5, waveIndex=2: hp = 10 + 2*5 = 20
}
public enum SpawnPattern { Circle, Rectangle }