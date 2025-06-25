using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f; // Tốc độ di chuyển
    [SerializeField] private float idealAttackDistance = 6f; // Khoảng cách lý tưởng để boss đứng yên và tấn công
    [SerializeField] private float chaseStartDistance = 8f; // Khoảng cách mà boss sẽ bắt đầu đuổi theo người chơi (nếu xa hơn idealAttackDistance)
    [SerializeField] private float stopChaseDistance = 5.5f; // Khoảng cách mà boss sẽ dừng đuổi (nếu gần hơn idealAttackDistance)
    [SerializeField] private float flipThreshold = 0.1f; // Ngưỡng để lật hình ảnh boss
    [SerializeField] private float damage = 20f; 



    // Biến công khai để BossProController có thể đọc trạng thái này
    public bool IsInAttackRange { get; private set; }

    private Transform player; // Tham chiếu tới người chơi
    private Rigidbody2D rb;
    private Animator animator; // Dùng cho hoạt ảnh di chuyển

    private float lastFacingDirectionX = 1f; // Lưu hướng cuối cùng của boss (1 cho phải, -1 cho trái)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        IsInAttackRange = false; // Khởi tạo ban đầu là không trong tầm tấn công
    }

    private void FixedUpdate() // Sử dụng FixedUpdate cho vật lý
    {
        if (player == null)
        {
            rb.velocity = Vector2.zero; // Dừng nếu không có người chơi
            SetAnimation(false); // Đặt hoạt ảnh đứng yên
            IsInAttackRange = false; // Đảm bảo trạng thái đúng
            return;
        }
        Movement();
        
    }
    void Movement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        Vector2 currentBossPos = transform.position;
        Vector2 moveDirection = Vector2.zero;
        bool shouldMove = false;

        // Nếu người chơi quá xa tầm tấn công lý tưởng
        if (distanceToPlayer > chaseStartDistance)
        {
            // Di chuyển lại gần người chơi
            moveDirection = ((Vector2)player.position - currentBossPos).normalized;
            shouldMove = true;
        }
        // Nếu người chơi quá gần tầm tấn công lý tưởng
        else if (distanceToPlayer < stopChaseDistance)
        {
            // Di chuyển ra xa người chơi
            moveDirection = (currentBossPos - (Vector2)player.position).normalized;
            shouldMove = true;
        }

        if (shouldMove)
        {
            rb.velocity = moveDirection * moveSpeed;
            SetAnimation(true); // Bật hoạt ảnh di chuyển
            Flip(moveDirection.x); // Lật boss theo hướng di chuyển
            IsInAttackRange = false; // Boss đang di chuyển, không trong tầm tấn công
        }
        else
        {
            // Boss đang trong khoảng cách lý tưởng để tấn công
            rb.velocity = Vector2.zero; // Dừng di chuyển
            SetAnimation(false); // Đặt hoạt ảnh đứng yên
            IsInAttackRange = true; // Boss đang trong tầm tấn công

            // Vẫn lật boss để nó nhìn về phía người chơi khi đứng yên
            Vector2 directionToPlayer = ((Vector2)player.position - currentBossPos).normalized;
            Flip(directionToPlayer.x);
        }
    }
     
    private void Flip(float horizontalInput)
    {
        if (horizontalInput > flipThreshold && lastFacingDirectionX < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Nhìn phải
            lastFacingDirectionX = 1;
        }
        else if (horizontalInput < -flipThreshold && lastFacingDirectionX > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Nhìn trái
            lastFacingDirectionX = -1;
        }
    }

    private void SetAnimation(bool isMoving)
    {
        animator?.SetBool("isMoving", isMoving);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
                Debug.Log($"Dealt {damage} damage to player");
            }
        }
    }
}
