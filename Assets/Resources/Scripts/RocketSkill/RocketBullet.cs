using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float speed = 5f;
    public float damage = 30f;
    public float lifetime = 7f;
    private Animator animator;
    private bool hasExploded = false;
    private float playerBaseDamageForExplosion = 0f;

    private Vector2 direction = Vector2.right;
    Rigidbody2D rb;
    public void SetDirection(Vector2 dir) => direction = dir.normalized;
    public void SetSpeed(float s) => speed = s;
    public void SetDamage(float d) => damage = d;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        // Tự động hủy sau 7 giây nếu không va chạm
        hasExploded = false;
        Invoke("AutoReturnToPool", lifetime);
    }
    private void OnDisable()
    {
        CancelInvoke("AutoReturnToPool"); // tránh gọi nhầm khi object đã bị disable
    }
    private void AutoReturnToPool()
    {
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }
    public void Activate()
    {
        rb.velocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    //private void Update()
    //{
    //    transform.Translate(direction * speed * Time.deltaTime);
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;
        if (collision.CompareTag("Enemy"))
        {
            //EnemyController enemy = collision.GetComponent<EnemyController>();
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
            if (animator != null)
            {
                hasExploded = true;
                rb.velocity = Vector2.zero;
                animator.SetTrigger("Explode");
            }

        }
    }
    public void SetPlayerBaseDamage(float baseDmg)
    {
        playerBaseDamageForExplosion = baseDmg;
    }

    public void OnExplosionAnimationEnd()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = ObjectPooling.Instance.GetFromPool(explosionPrefab, transform.position, Quaternion.identity);
            ExplosionZone explosionScript = explosion.GetComponent<ExplosionZone>();
            if (explosionScript != null)
            {
                explosionScript.SetDamage(PlayerSkillManager.Instance.explosionDamage);
                explosionScript.SetExplosionScale(PlayerSkillManager.Instance.explosionScale);
                explosionScript.SetPlayerBaseDamage(playerBaseDamageForExplosion);
                explosionScript.PlayExplosionSound();
            }
        }

        ObjectPooling.Instance.ReturnToPool(gameObject);
    }
}
