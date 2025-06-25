using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable,IBoss
{
    public float maxHP = 100f;
    private float currentHP;

    public event Action OnDeath;
    private void OnEnable()
    {
        EnemyManage.instance?.Register(transform);
    }
    private void OnDisable()
    {
        EnemyManage.instance?.Unregister(transform);
        OnDeath = null;
    }
    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        //Debug.Log("nhận " + damage);
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // animation, hiệu ứng, rồi destroy hoặc ẩn
        if (OnDeath != null) // Kiểm tra xem có subscriber nào không
        {
            OnDeath.Invoke(); // <--- KÍCH HOẠT EVENT Ở ĐÂY
        }
        Debug.Log("Boss died!");
        ObjectPooling.Instance.ReturnToPool(gameObject);
      
    }
}
