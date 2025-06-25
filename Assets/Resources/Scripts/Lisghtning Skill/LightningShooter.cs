using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningShooter : MonoBehaviour
{
    public GameObject lightningPrefab;
    public float shootInterval = 5f;
    public float damage = 2f;
    public float radius = 1f;

    private float timer;
    PlayerDame playerDame;
    private void Awake()
    {
        playerDame = GetComponent<PlayerDame>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            ShootLightning();
            timer = 0f;
        }
    }

    void ShootLightning()
    {
        float extraDame = playerDame != null ? playerDame.GetCurrentDamage() : 0f;
        float finalDamage = damage + extraDame;
        Transform target = EnemyManage.instance.GetRandomEnemy();
        if (target == null || lightningPrefab == null) return;

        GameObject lightning = ObjectPooling.Instance.GetFromPool(lightningPrefab, target.position, Quaternion.identity);
        LightningStrike strike = lightning.GetComponent<LightningStrike>();
        if (strike != null)
        {
            strike.Init(finalDamage, radius);
        }
        AudioManager.Instance.PlayPlayerLighting();
    }
    public void UpdateLightningStats(int newDamage)
    {
        damage = newDamage;
    }
}
