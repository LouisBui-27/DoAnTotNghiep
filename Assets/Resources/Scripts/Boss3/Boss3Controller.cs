using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Controller : MonoBehaviour
{
    [Header("Phase 1 - Lunge")]
    [SerializeField] private float lungeForce = 10f;
    [SerializeField] private float lungeCooldown = 2f;
    private bool canLunge = true;

    [Header("Phase 2 - Throw Rock")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private int rocksPerThrow = 5;
    [SerializeField] private float throwCooldown = 3f;
    [SerializeField] private float rockSpeed = 5f;
    [SerializeField] private int throwCountBeforeLunge = 3;
    private bool isPhase2 = false;
    private bool canThrow = true;
    private int currentThrowCount = 0;
    private bool isDoingLunge = false;

    [Header("References")]
    [SerializeField] private Transform player;
    private Rigidbody2D rb;
    private Boss3Health health;
    private Animator animator;

    [Header("Telegraph Settings")]
    [SerializeField] private LineRenderer lungeLineRenderer;
    [SerializeField] private Gradient lungeWarningGradient;
    [SerializeField] private float lineWidth = 0.2f;
    [SerializeField] private float lineAnimationSpeed = 2f;
    [SerializeField] private Material lungeWarningMaterial;
    [SerializeField] private int lineSegments = 20;
    [SerializeField] private float lineCurveHeight = 1f;
     [SerializeField] private float telegraphTime = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Boss3Health>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }

        // Make sure health exists
        if (health == null)
        {
            Debug.LogError("Boss3Health component is missing on " + gameObject.name);
        }
        InitializeLineRenderer();
    }

    private void InitializeLineRenderer()
    {
        if (lungeLineRenderer == null)
        {
            GameObject lineObj = new GameObject("LungeWarningLine");
            lineObj.transform.SetParent(transform);
            lungeLineRenderer = lineObj.AddComponent<LineRenderer>();
        }

        lungeLineRenderer.positionCount = lineSegments;
        lungeLineRenderer.widthCurve = AnimationCurve.Linear(0, lineWidth, 1, lineWidth);
        lungeLineRenderer.colorGradient = lungeWarningGradient;
        lungeLineRenderer.material = lungeWarningMaterial;
        lungeLineRenderer.textureMode = LineTextureMode.Tile;
        lungeLineRenderer.enabled = false;
    }

    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player not found. Boss behavior will be limited.");
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // Quay mặt về hướng nhân vật
        FacePlayer();
        if (animator != null)
        {
            bool isMoving = rb.velocity.magnitude > 0.1f;
            animator.SetBool("isMoving", isMoving);
        }
        if (!isPhase2)
        {
            // Phase 1: Chỉ lao vào người chơi
            if (canLunge)
                StartCoroutine(LungeAtPlayer());
        }
        else
        {
            // Phase 2: Kết hợp ném đá và lao vào
            if (!isDoingLunge)
            {
                if (canThrow)
                    StartCoroutine(ThrowRocks());
            }
        }
    }

    private void FacePlayer()
    {
        // Kiểm tra vị trí của player so với boss
        Vector3 direction = player.position - transform.position;

        // Nếu player đang ở bên phải boss
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        // Nếu player đang ở bên trái boss
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    IEnumerator LungeAtPlayer()
    {
        canLunge = false;

        // Hiển thị cảnh báo đường lao
        yield return StartCoroutine(AnimateLungeWarning());

        // Thực hiện lao
        Vector2 targetPos = player.position;
        Vector2 startPos = transform.position;
        Vector2 direction = (targetPos - startPos).normalized;

        float distance = Vector2.Distance(startPos, targetPos);
        float travelTime = distance / lungeForce;

        float timer = 0f;

        while (timer < travelTime)
        {
            rb.velocity = direction * lungeForce;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(lungeCooldown);
        canLunge = true;
    }
    IEnumerator AnimateLungeWarning()
    {
        if (player == null || lungeLineRenderer == null) yield break;

        lungeLineRenderer.enabled = true;
        float animationTimer = 0f;

        while (animationTimer < telegraphTime)
        {
            // Cập nhật vị trí đường cong
            UpdateCurvedLine(transform.position, player.position, animationTimer / telegraphTime);

            // Thay đổi màu sắc/animation
            float alpha = Mathf.PingPong(animationTimer * lineAnimationSpeed, 1f);
            Gradient newGradient = new Gradient();
            newGradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.red, 0f),
                    new GradientColorKey(Color.yellow, 0.5f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(alpha, 0f),
                    new GradientAlphaKey(alpha * 0.5f, 1f)
                }
            );
            lungeLineRenderer.colorGradient = newGradient;

            animationTimer += Time.deltaTime;
            yield return null;
        }

        lungeLineRenderer.enabled = false;
    }

    private void UpdateCurvedLine(Vector3 start, Vector3 end, float progress)
    {
        Vector3[] points = new Vector3[lineSegments];
        Vector3 Dir = (end - start).normalized;
        float offsetDistance = .75f;
        Vector3 offSetStart = start + Dir * offsetDistance;
        for (int i = 0; i < lineSegments; i++)
        {
            float t = i / (float)(lineSegments - 1);
            points[i] = Vector3.Lerp(offSetStart, end, t);
        }

        lungeLineRenderer.positionCount = lineSegments;
        lungeLineRenderer.SetPositions(points);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    IEnumerator ThrowRocks()
    {
        canThrow = false;

        // Telegraph attack
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < rocksPerThrow; i++)
        {
            float angle = i * (360f / rocksPerThrow);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, dir);
            // Check if ObjectPooling exists
            GameObject rock = null;

            if (ObjectPooling.Instance != null)
            {
                rock = ObjectPooling.Instance.GetFromPool(rockPrefab, transform.position, rotation);
            }
            else
            {
                rock = Instantiate(rockPrefab, transform.position, rotation);
            }

            if (rock != null)
            {
                Rigidbody2D rbRock = rock.GetComponent<Rigidbody2D>();
                if (rbRock != null)
                {
                    rbRock.velocity = dir * rockSpeed;
                }
            }

            //yield return new WaitForSeconds(0.1f);
        }

        // Tăng số lần ném đá
        currentThrowCount++;

        // Nếu đã ném đủ số lần quy định, chuyển sang lao vào người chơi
        if (currentThrowCount >= throwCountBeforeLunge)
        {
            currentThrowCount = 0;
            StartCoroutine(Phase2Lunge());
        }
        else
        {
            // Cooldown giữa các lần ném đá
            yield return new WaitForSeconds(throwCooldown);
            canThrow = true;
        }
    }

    IEnumerator Phase2Lunge()
    {
        isDoingLunge = true;

        // Đợi một chút trước khi lao vào (telegraph)
        yield return StartCoroutine(AnimateLungeWarning());

        // Lao vào người chơi
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * lungeForce;

        // Duy trì tốc độ trong một khoảng thời gian
        yield return new WaitForSeconds(0.5f);

        // Dừng lại
        rb.velocity = Vector2.zero;

        // Cooldown sau khi lao vào
        yield return new WaitForSeconds(1f);

        // Quay lại ném đá
        isDoingLunge = false;
        canThrow = true;
    }

    public void OnHealthChanged(float hpPercent)
    {
        if (!isPhase2 && hpPercent <= 0.5f)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        rb.velocity = Vector2.zero;

        // Cancel any ongoing coroutines when entering phase 2
        StopAllCoroutines();

        // Optional: Play phase transition effect
        Debug.Log("Boss entering Phase 2!");

        // Reset các biến trạng thái
        currentThrowCount = 0;
        isDoingLunge = false;

        // Reset ability cooldowns for immediate phase 2 action
        canThrow = true;
    }
}
