using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public float lifetime = 3f;
    public int damage = 10;
    public GameObject explosionPrefab;
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 dir)
    {
        rb.velocity = dir.normalized * speed;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Đặt góc xoay cho transform của viên đạn
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
        // Tắt đạn thay vì hủy
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // EnemyController enemy = collision.GetComponent<EnemyController>();
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                //SpawnExplosion();
            }

            ObjectPooling.Instance.ReturnToPool(gameObject);
        }
    }
    void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            // Sử dụng Object Pooling cho hiệu ứng nổ nếu có
            ObjectPooling.Instance.GetFromPool(explosionPrefab, transform.position, Quaternion.identity);
            // Hoặc Instantiate nếu không có Object Pooling:
            // Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
