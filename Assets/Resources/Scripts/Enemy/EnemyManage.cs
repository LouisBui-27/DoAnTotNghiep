using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManage : MonoBehaviour
{
    public static EnemyManage instance;
    private List<Transform> enemies = new List<Transform>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
            Destroy(gameObject);    
    }

    public void Register(Transform enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }
    public void Unregister(Transform enemy)
    {
        enemies.Remove(enemy);
    }
    public Transform FindClosestEnemy(Vector3 fromPosition, float maxRange)
    {
        Transform closest = null;
        float minDist = maxRange;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(fromPosition, enemy.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
    public Transform GetRandomEnemy()
    {
        enemies.RemoveAll(e => e == null); // lọc bớt những thằng đã bị destroy

        if (enemies.Count == 0) return null;
        int index = Random.Range(0, enemies.Count);
        return enemies[index];
    }
}
