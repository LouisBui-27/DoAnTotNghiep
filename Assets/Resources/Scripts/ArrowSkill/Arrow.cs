using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage = 10;
    public float speed = 10f;

    private Vector2 direction = Vector2.right;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void SetDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            direction = dir.normalized;
    }
    public void Activate()
    {
        rb.velocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    private void OnEnable()
    {
        // Khi đạn được kích hoạt, đặt lại thời gian tồn tại
        Invoke("DeactivateBullet", lifetime);
    }

    private void OnDisable()
    {
        // Hủy bỏ lời gọi Invoke khi đạn bị tắt
        CancelInvoke("DeactivateBullet");
    }

    private void DeactivateBullet()
    {
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }
    //private void Update()
    //{
    //    transform.Translate(direction * speed * Time.deltaTime);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
           // EnemyController enemy = collision.GetComponent<EnemyController>();
            IDamageable enemy = collision.GetComponent<IDamageable>();
            if (enemy != null)
            {
                float critChance = PlayerSkillManager.Instance.critChance;
                float finalDamage = damage;

                if (Random.value < critChance)
                {
                    finalDamage *= 2f; // x2 damage cho chí mạng
                    Debug.Log("💥 Chí mạng! Gây " + finalDamage + " sát thương.");
                }

                enemy.TakeDamage(finalDamage);
            }

            ObjectPooling.Instance.ReturnToPool(gameObject);
        }
    }
}
