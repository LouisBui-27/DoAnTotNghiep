using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangShooter : MonoBehaviour
{
    public GameObject boomerangPrefab;
    public float shootInterval = 3f;
    public float damage = 20f; 
    public float returnSpeed = 10f;
    public float moveSpeed = 7f;
    public float maxDistance = 5f;

    private float timer;
    private PlayerDame playerDame;

    private void Awake()
    {
        playerDame = GetComponent<PlayerDame>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            ShootBoomerang();
            timer = 0f;
        }
    }
    void ShootBoomerang()
    {
        AudioManager.Instance.PlayPlayerBoomerang();
        GameObject boomObj = ObjectPooling.Instance.GetFromPool(boomerangPrefab, transform.position, Quaternion.identity);
        Boomerang boomerang = boomObj.GetComponent<Boomerang>();
        Transform closestEnemy = FindClosestEnemy(5f);
        float extraDamage = playerDame != null ? playerDame.GetCurrentDamage() : 0f;
        boomerang.damage = damage + extraDamage;

        boomerang.Init(transform, closestEnemy , moveSpeed,returnSpeed, maxDistance);
    }
    Transform FindClosestEnemy(float range)
    {
        return EnemyManage.instance.FindClosestEnemy(transform.position,range);
    }
}
