using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 moveInput;
    public Joystick joystick;
    private Vector2 lastMoveDirection; 
    private SpriteRenderer spriteRenderer;
    private float originalSpeed;
    private float speedBoostMultiplier = 1f;
    private float speedUpgrade = 1f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Lấy SpriteRenderer để flip nhân vật
        lastMoveDirection = Vector2.right;
        originalSpeed = speed;
        joystick = FindObjectOfType<Joystick>();
        if (joystick == null)
        {
            Debug.LogError("Không tìm thấy Joystick trong scene!");
        }
    }

    private void Update()
    {
        moveInput.x = joystick.Horizontal;
        moveInput.y = joystick.Vertical;

        moveInput = moveInput.normalized;

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput; 
        }

        FlipCharacter();
    }
    private void FixedUpdate()
    {
        rb.velocity = moveInput * speed * speedBoostMultiplier * speedUpgrade;
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        collision.GetComponent<EnemyController>().TakeDamage(5);
    //    }
    //}
    private void FlipCharacter()
    {
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false; // Đi sang phải -> không flip
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true; // Đi sang trái -> flip
        }
    }

    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection; // Lấy hướng di chuyển cuối cùng
    }
    public void ActivateSpeedBoost(float multiplier)
    {
        speedBoostMultiplier = multiplier;
    }
    public void upgradeSpeed(float multiplier)
    {
        speedUpgrade += multiplier;
    }
}
