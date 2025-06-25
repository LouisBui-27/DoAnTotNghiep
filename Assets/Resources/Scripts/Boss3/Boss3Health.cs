using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Health : MonoBehaviour, IDamageable, IBoss
{
    [SerializeField] private float maxHP = 100f;
    private float currentHP;
    private float contactDamage = 20f;
    private Boss3Controller bossController;

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
    void Awake()
    {
        currentHP = maxHP;
        bossController = GetComponent<Boss3Controller>();
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        float hpPercent = currentHP / maxHP;

        // Gọi boss kiểm tra phase nếu có
        if (bossController != null)
        {
            bossController.OnHealthChanged(hpPercent);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(contactDamage);

                // Optional: Knockback effect
                Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(knockbackDir * 10f, ForceMode2D.Impulse);
                }
            }
        }
    }
    private void Die()
    {
        if (OnDeath != null) // Kiểm tra xem có subscriber nào không
        {
            OnDeath.Invoke(); // <--- KÍCH HOẠT EVENT Ở ĐÂY
        }
        Debug.Log("Boss3 died!");
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }
}
