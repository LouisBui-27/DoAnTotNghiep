using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingObject : MonoBehaviour
{
    private Transform center;
    private float angle;
    private float radius;
    private float rotateSpeed;
    private float damage;

    public void Initialize(Transform centerPoint, float startAngle, float radiusValue, float speed, float dmg)
    {
        center = centerPoint;
        angle = startAngle * Mathf.Rad2Deg;
        radius = radiusValue;
        rotateSpeed = speed;
        damage = dmg;
    }

    void Update()
    {
        if (center == null) return;

        angle += rotateSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        transform.position = center.position + offset;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
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
                AudioManager.Instance.PlayPlayerOrbitingObject();
            }
        }
    }
}
