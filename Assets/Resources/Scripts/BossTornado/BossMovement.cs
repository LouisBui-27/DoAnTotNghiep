using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 7f;
    [SerializeField] private float changeDirectionInterval = 2f;

    [Header("References")]
    [SerializeField] private BossController bossController;

    private Transform player;
    private Rigidbody2D rb;
    private float directionTimer;
    private Vector2 currentDirection;
    private bool isSkillActive;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        directionTimer = changeDirectionInterval;
        ChooseNewDirection();
    }

    private void Update()
    {
        // Cập nhật trạng thái skill từ BossController
        isSkillActive = bossController != null && bossController.IsCastingSkill2;
    }
    private void FixedUpdate()
    {
        if (player == null || isSkillActive)
        {
            rb.velocity = Vector2.zero;
           // SetAnimation(false, false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Logic di chuyển mới
        directionTimer -= Time.fixedDeltaTime;

        if (directionTimer <= 0 || distanceToPlayer < minDistance || distanceToPlayer > maxDistance)
        {
            ChooseNewDirection();
            directionTimer = changeDirectionInterval;
        }

        rb.velocity = currentDirection * moveSpeed;
        Flip();
        SetAnimation(rb.velocity.magnitude > 0.1f, false);
    }

    private void ChooseNewDirection()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < minDistance)
        {
            // Di chuyển ra xa player
            currentDirection = (transform.position - player.position).normalized;
        }
        else if (distanceToPlayer > maxDistance)
        {
            // Tiến lại gần player
            currentDirection = (player.position - transform.position).normalized;
        }
        else
        {
            // Di chuyển ngẫu nhiên xung quanh player
            Vector2 toPlayer = (player.position - transform.position).normalized;
            Vector2 perpendicular = new Vector2(-toPlayer.y, toPlayer.x);
            currentDirection = Vector2.Lerp(toPlayer, perpendicular, Random.Range(-0.5f, 0.5f)).normalized;
        }
    }
    private void Flip()
    {
        if (currentDirection.x > 0.01f)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z); // nhìn phải
        }
        else if (currentDirection.x < -0.01f)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z); // nhìn trái
        }
    }
    public void StopForSkill(float duration)
    {
        StartCoroutine(StopMovementCoroutine(duration));
    }

    private IEnumerator StopMovementCoroutine(float duration)
    {
        isSkillActive = true;
        rb.velocity = Vector2.zero;
        SetAnimation(false, true);
        yield return new WaitForSeconds(duration);
        isSkillActive = false;
        SetAnimation(false, false);
    }
    private void SetAnimation(bool isMoving, bool isAttacking)
    {
        animator?.SetBool("isMoving", isMoving);
        animator?.SetBool("isAttacking", isAttacking);
    }
}

