using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletProjectilePrefab; // Prefab của viên đạn (BulletProjectile)
    public Transform firePoint; // Điểm mà đạn sẽ được bắn ra
    public float fireRate = 1f; // Tần suất bắn (số viên đạn mỗi giây)
    public int bulletDamage = 10; // Sát thương của đạn (truyền cho BulletProjectile)

    [Header("Targeting")]
    private Transform player; // Tham chiếu tới người chơi
    private Animator animator;

    // Thay vì dùng nextFireTime trong Update(), chúng ta sẽ dùng Coroutine
    private BossProMovement bossMovement;
    private Coroutine shootingCoroutine; // Biến để lưu trữ Coroutine bắn đạn
    private bool canShootFromAnimationEvent = true;
    [SerializeField] private float attackAnimationDuration = .2f;

    private void Start()
    {
        // Tìm người chơi khi game bắt đầu
        player = GameObject.FindWithTag("Player")?.transform;
        bossMovement = GetComponent<BossProMovement>();
        animator = GetComponent<Animator>();
        // Kiểm tra các tham chiếu cần thiết
        if (firePoint == null)
        {
            Debug.LogError("Fire Point is not assigned on BossProController of " + gameObject.name + "!");
        }
        if (bulletProjectilePrefab == null)
        {
            Debug.LogError("Bullet Projectile Prefab is not assigned on BossProController of " + gameObject.name + "!");
        }

        // Bắt đầu Coroutine bắn đạn
        if (player != null && firePoint != null && bulletProjectilePrefab != null)
        {
            shootingCoroutine = StartCoroutine(CheckAndManageShootingRoutine());
        }
    }

    private void OnDisable()
    {
        // Khi boss bị tắt hoặc hủy, dừng Coroutine để tránh lỗi
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
    }
    IEnumerator CheckAndManageShootingRoutine()
    {
        while (true)
        {
            if (bossMovement.IsInAttackRange)
            {
                // Khi boss trong tầm tấn công, bật hoạt ảnh tấn công
                SetAnimationAttacking(true);
                canShootFromAnimationEvent = true; // Cho phép Animation Event kích hoạt bắn
                yield return new WaitForSeconds(attackAnimationDuration);
                SetAnimationAttacking(false);
                float remainingCooldown = (fireRate) - attackAnimationDuration;
                if (remainingCooldown < 0) remainingCooldown = 0; // Đảm bảo không chờ thời gian âm

                // Chờ cho đến khi hoạt ảnh tấn công hoàn thành hoặc chuyển sang trạng thái khác
                // Với Has Exit Time, Animator sẽ tự chuyển về Idle khi Attack xong
                // Chúng ta chỉ cần đảm bảo rằng boss ở trong tầm tấn công
                yield return new WaitForSeconds(remainingCooldown); // Chờ đủ thời gian cho hoạt ảnh và cooldown bắn
            }
            else
            {
                // Nếu boss đang di chuyển, đảm bảo hoạt ảnh tấn công tắt
                SetAnimationAttacking(false);
                canShootFromAnimationEvent = false; // Ngăn không cho Animation Event bắn khi đang di chuyển
                yield return null; // Chờ một khung hình trước khi kiểm tra lại
            }
        }
    }

    // PHƯƠNG THỨC MỚI SẼ ĐƯỢC GỌI BỞI ANIMATION EVENT
    public void OnAttackFrame()
    {
        // Chỉ bắn nếu boss đang trong tầm tấn công và cờ cho phép bắn được bật
        if (bossMovement.IsInAttackRange && canShootFromAnimationEvent)
        {
            ShootBullet(); // Gọi phương thức bắn đạn thực tế
            canShootFromAnimationEvent = false; // Tắt cờ để ngăn bắn nhiều lần trong cùng một hoạt ảnh
            Debug.Log("Bullet fired via Animation Event!");
        }
    }

    // Tách riêng logic bắn đạn ra một phương thức riêng
    void ShootBullet()
    {
        if (bulletProjectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Bullet Projectile Prefab or Fire Point is not assigned!");
            return;
        }

        GameObject bulletGO = ObjectPooling.Instance.GetFromPool(bulletProjectilePrefab, firePoint.position, Quaternion.identity);
        BulletProjectile bullet = bulletGO.GetComponent<BulletProjectile>();

        if (bullet != null)
        {
            Vector2 directionToPlayer = (player.position - firePoint.position).normalized;
            bullet.SetDirection(directionToPlayer);
            bullet.damage = bulletDamage;
        }
        else
        {
            Debug.LogError("BulletProjectile script not found on the bullet prefab!");
            ObjectPooling.Instance.ReturnToPool(bulletGO);
        }
    }

    private void SetAnimationAttacking(bool isAttacking)
    {
        animator?.SetBool("isAttacking", isAttacking);
    }
}
