using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = .5f;
    public float attackInterval = 1.5f;
    public float damage = 10f;
    public float searchRange = 5f;
    public float maxHealth = 100f;

    private float currentHealth;
    private float attackTimer;
    private Transform owner;
    private Transform target;
    private Rigidbody2D rb;
    public float followDistance = 3f; // Khoảng cách giữa pet và chủ

    public void SetOwner(Transform ownerTransform)
    {
        owner = ownerTransform;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            // Nếu kẻ địch trong phạm vi tấn công, dừng lại và tấn công
            if (Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                rb.velocity = Vector2.zero; // Dừng lại khi tấn công
                AttackEnemy();
            }
            else
            {
                // Nếu không trong phạm vi tấn công, di chuyển tới kẻ địch
                MoveTowardsTarget();
            }
        }
        else
        {
            // Nếu không có kẻ địch, quay lại đi theo chủ
            FollowOwner();
        }
    }
    void MoveTowardsTarget()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }
    void FindNearestEnemy()
    {
        target = EnemyManage.instance.FindClosestEnemy(transform.position, attackRange);
    }
    void FollowOwner()
    {
        if (owner == null) return;

        // Tính toán khoảng cách cố định giữa pet và chủ nhân
        Vector2 desiredPosition = (owner.position - transform.position).normalized * followDistance;
        Vector2 targetPosition = (Vector2)owner.position + desiredPosition;

        // Di chuyển pet đến vị trí mong muốn
        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = dir * moveSpeed;
    }
    private void Update()
    {
        if (target == null)
        {
            FindNearestEnemy(); // Tìm kiếm mục tiêu mới khi không có kẻ địch
        }
    }

    void AttackEnemy()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        if (target != null)
        {
            EnemyController enemy = target.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                attackTimer = attackInterval;
            }
            else
            {
                // Nếu enemy đã chết hoặc null
                target = null;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Destroy(gameObject); // hoặc ObjectPooling.Instance.ReturnToPool(gameObject);
        }
    }


}
