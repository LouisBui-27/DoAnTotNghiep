using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float circleRadius = 2.5f; // Bán kính vòng tròn khi di chuyển
    [SerializeField] private float circleSpeed = 1.5f; // Tốc độ di chuyển theo vòng tròn
    [SerializeField] private float dashSpeed = 6f; // Tốc độ lao nhanh
    [SerializeField] private float dashDuration = 0.5f; // Thời gian lao nhanh
    [SerializeField] private float dashCooldown = 3f; // Thời gian hồi lao nhanh
    [SerializeField] private float repositionDistance = 1.5f; // Khoảng cách xem xét vị trí mới
    [SerializeField] private float idleTime = 0.5f; // Thời gian đứng im giữa các hành động

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float phase2Speed = 1.5f;

    [Header("Phase 2 Settings")]
    [SerializeField] private float bulletInterval = 0.5f;
    [SerializeField] private float minionSpawnInterval = 5f;
    [SerializeField] private int minionsPerSpawn = 2;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private Boss2Attack attackSystem;

    // Movement variables
    private Vector2 moveDirection;
    private Vector2 targetPosition;
    private bool isRepositioning = false;
    private bool isCircling = false;
    private bool isDashing = false;
    private bool canDash = true;
    private float circleAngle = 0f;
    private float timeInCurrentState = 0f;

    // State management
    private enum BossState { Idle, Chase, Circle, Attack, Dash, Reposition }
    private BossState currentState = BossState.Idle;
    private float stateTimer = 0f;

    // Other variables
    private bool isPhase2 = false;
    private bool canAttack = true;
    private Coroutine bulletCoroutine;
    private Coroutine minionCoroutine;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        // Thêm animation parameter nếu cần
        if (animator != null)
        {
            // Thêm các parameters cho animator
            animator.SetBool("IsMoving", false);
        }
    }

    private void Update()
    {
        if (player == null) return;

        stateTimer += Time.deltaTime;

        // Cập nhật trạng thái
        UpdateState();

        // Xử lý hành vi dựa trên trạng thái
        HandleState();

        // Luôn quay mặt về phía người chơi
        FacePlayer();
    }

    void FixedUpdate()
    {
        // Sử dụng Rigidbody2D để di chuyển nhân vật
        if (currentState != BossState.Idle && currentState != BossState.Attack && !isDashing)
        {
            rb.velocity = moveDirection * moveSpeed;
        }
        else if (isDashing)
        {
            rb.velocity = moveDirection * dashSpeed;
        }
    }

    void UpdateState()
    {
        if (isPhase2)
        {
            UpdatePhase2State();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Chuyển đổi trạng thái dựa trên khoảng cách và timeInCurrentState
        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer > idleTime)
                {
                    if (distance > chaseRange)
                    {
                        ChangeState(BossState.Chase);
                    }
                    else if (distance <= attackRange && canAttack)
                    {
                        ChangeState(BossState.Attack);
                    }
                    else
                    {
                        // Quyết định xem có nên di chuyển theo vòng tròn, lao nhanh hay đổi vị trí
                        float decision = Random.value;
                        if (decision < 0.4f && canDash)
                        {
                            ChangeState(BossState.Dash);
                        }
                        else if (decision < 0.7f)
                        {
                            ChangeState(BossState.Circle);
                        }
                        else
                        {
                            ChangeState(BossState.Reposition);
                        }
                    }
                }
                break;

            case BossState.Chase:
                if (distance <= chaseRange)
                {
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Circle:
                if (stateTimer > 3f || distance > chaseRange || distance <= attackRange)
                {
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Attack:
                // State Attack sẽ được xử lý trong coroutine MeleeAttack
                break;

            case BossState.Dash:
                // State Dash sẽ được xử lý trong coroutine Dash
                break;

            case BossState.Reposition:
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f || stateTimer > 2f)
                {
                    ChangeState(BossState.Idle);
                }
                break;
        }
    }

    void UpdatePhase2State()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer > idleTime)
                {
                    float decision = Random.value;
                    if (distance > chaseRange)
                    {
                        ChangeState(BossState.Chase);
                    }
                    else if (decision < 0.3f && canDash)
                    {
                        ChangeState(BossState.Dash);
                    }
                    else if (decision < 0.7f)
                    {
                        ChangeState(BossState.Circle);
                    }
                    else
                    {
                        ChangeState(BossState.Reposition);
                    }
                }
                break;

            case BossState.Chase:
                if (distance <= chaseRange)
                {
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Circle:
                if (stateTimer > 4f)
                {
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Dash:
                // State Dash sẽ được xử lý trong coroutine Dash
                break;

            case BossState.Reposition:
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f || stateTimer > 2f)
                {
                    ChangeState(BossState.Idle);
                }
                break;
        }
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        // Thiết lập các giá trị ban đầu cho trạng thái mới
        switch (newState)
        {
            case BossState.Idle:
                rb.velocity = Vector2.zero;
                if (animator != null) animator.SetBool("IsMoving", false);
                break;

            case BossState.Chase:
                if (animator != null) animator.SetBool("IsMoving", true);
                break;

            case BossState.Circle:
                circleAngle = Random.Range(0, 2 * Mathf.PI);
                isCircling = true;
                if (animator != null) animator.SetBool("IsMoving", true);
                break;

            case BossState.Attack:
                rb.velocity = Vector2.zero;
                if (animator != null) animator.SetBool("IsMoving", false);
                StartCoroutine(MeleeAttack());
                break;

            case BossState.Dash:
                if (canDash)
                {
                    StartCoroutine(Dash());
                }
                else
                {
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Reposition:
                FindRepositionTarget();
                if (animator != null) animator.SetBool("IsMoving", true);
                break;
        }
    }

    void HandleState()
    {
        switch (currentState)
        {
            case BossState.Idle:
                // Không làm gì, đứng yên
                moveDirection = Vector2.zero;
                break;

            case BossState.Chase:
                ChasePlayer();
                break;

            case BossState.Circle:
                CircleAroundPlayer();
                break;

            case BossState.Reposition:
                MoveToPosition(targetPosition);
                break;

                // Attack và Dash đã được xử lý riêng trong coroutine
        }
    }

    void ChasePlayer()
    {
        moveDirection = (player.position - transform.position).normalized;
    }

    void CircleAroundPlayer()
    {
        // Di chuyển theo một đường tròn xung quanh người chơi
        circleAngle += circleSpeed * Time.deltaTime;

        float x = Mathf.Cos(circleAngle) * circleRadius;
        float y = Mathf.Sin(circleAngle) * circleRadius;

        Vector2 circleOffset = new Vector2(x, y);
        Vector2 targetPos = (Vector2)player.position + circleOffset;

        moveDirection = (targetPos - (Vector2)transform.position).normalized;
    }

    void MoveToPosition(Vector2 position)
    {
        moveDirection = (position - (Vector2)transform.position).normalized;
    }

    void FindRepositionTarget()
    {
        // Tìm một vị trí random xung quanh người chơi để di chuyển đến
        float randomAngle = Random.Range(0, 2 * Mathf.PI);
        float randomDistance = Random.Range(attackRange, chaseRange);

        Vector2 offset = new Vector2(
            Mathf.Cos(randomAngle) * randomDistance,
            Mathf.Sin(randomAngle) * randomDistance
        );

        targetPosition = (Vector2)player.position + offset;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

       // if (animator != null) animator.SetTrigger("Dash");

        // Lao nhanh về phía người chơi
        moveDirection = (player.position - transform.position).normalized;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.velocity = Vector2.zero;
        ChangeState(BossState.Idle);

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }

    void FacePlayer()
    {
        if (player == null) return;

        float dirX = player.position.x - transform.position.x;

        if (Mathf.Abs(dirX) > 0.1f)
        {
            float scaleX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(Mathf.Sign(dirX) * scaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    IEnumerator MeleeAttack()
    {
        canAttack = false;

        if (IsPlayerInFront())
        {
            if (animator != null) animator.SetTrigger("Attack");

            // Bật Collider tấn công
            attackSystem.EnableAttack();
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        ChangeState(BossState.Idle);
    }

    bool IsPlayerInFront()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * 1f, -0.5f); // điểm giữa box, cách boss 1 đơn vị theo hướng nhìn
        Vector2 boxSize = new Vector2(1f, 0.5f); // kích thước vùng kiểm tra (rộng x cao)
        float angle = 0f;
        LayerMask playerLayer = LayerMask.GetMask("Player");

        Collider2D hit = Physics2D.OverlapBox(origin, boxSize, angle, playerLayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, circleRadius);
        }

        Vector2 origin = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * 1f, -0.5f);
        Vector2 boxSize = new Vector2(1f, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
    }

    public void CheckPhase(float hpPercent)
    {
        if (!isPhase2 && hpPercent <= 0.5f)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        moveSpeed = phase2Speed;

        // Bắt đầu các coroutine
        bulletCoroutine = StartCoroutine(ShootBulletLoop());
        minionCoroutine = StartCoroutine(SummonMinionLoop());

        // Animation chuyển phase
       // if (animator != null) animator.SetTrigger("PhaseTransition");
    }

    IEnumerator ShootBulletLoop()
    {
        while (isPhase2)
        {
            ShootBullet();
            yield return new WaitForSeconds(bulletInterval);
        }
    }

    void ShootBullet()
    {
        if (bulletPrefab != null && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject bullet = ObjectPooling.Instance.GetFromPool(bulletPrefab, transform.position, Quaternion.identity);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(direction);
            }
        }
    }

    IEnumerator SummonMinionLoop()
    {
        while (isPhase2)
        {
            yield return new WaitForSeconds(minionSpawnInterval);
            SummonMinions();
        }
    }

    void SummonMinions()
    {
        if (minionPrefab != null)
        {
            for (int i = 0; i < minionsPerSpawn; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                ObjectPooling.Instance.GetFromPool(minionPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    private void OnDisable()
    {
        if (bulletCoroutine != null) StopCoroutine(bulletCoroutine);
        if (minionCoroutine != null) StopCoroutine(minionCoroutine);
    }
}
