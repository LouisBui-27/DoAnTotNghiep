using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    private float damage;
    private float radius;

    public void Init(float damage, float radius)
    {
        this.damage = damage;
        this.radius = radius;

        StartCoroutine(StrikeRoutine());
    }

    IEnumerator StrikeRoutine()
    {
        yield return null; // chờ 1 frame để chắc chắn object đã spawn xong
       // Strike();
        yield return new WaitForSeconds(0.5f); // delay để hiệu ứng hiển thị
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
           // EnemyController enemy = other.GetComponent<EnemyController>();
           IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float critChance = PlayerSkillManager.Instance.critChance;
                float finalDamage = damage;

                if (Random.value < critChance)
                {
                    finalDamage *= 2f; // x2 damage cho chí mạng
                    Debug.Log("💥 Chí mạng! Gây " + finalDamage + " sát thương.");
                }

                damageable.TakeDamage(finalDamage);

            }
        }
    }
    //void Strike()
    //{
    //    float scaledRadius = transform.localScale.x * 0.5f;
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, scaledRadius);
    //    foreach (var hit in hits)
    //    {
    //        if (hit.CompareTag("Enemy"))
    //        {
    //            EnemyController enemy = hit.GetComponent<EnemyController>();
    //            if (enemy != null)
    //            {
    //                enemy.TakeDamage(damage);
    //            }
    //        }
    //    }
    //}
  
}
