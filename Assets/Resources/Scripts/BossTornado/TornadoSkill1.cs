using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSkill1 : TornadoSkillBase
{
    public float speed = 3f;
    public float lifeTime = 6f;

    private Transform target;
    private float timer;

    public float damage = 5f;


    protected override void OnEnable()
    {
        base.OnEnable(); // Call base class implementation
        target = GameObject.FindWithTag("Player")?.transform;
        timer = 0f;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            transform.position += (Vector3)dir * speed * Time.deltaTime;
        }

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ObjectPooling.Instance.ReturnToPool(gameObject); // hoặc ReturnToPool nếu dùng Pool
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isImpacted) return;

        // Xử lý damage
        IDamageable target = other.GetComponent<IDamageable>();
        target?.TakeDamage(damage);

        HandleImpact();
    }
}
