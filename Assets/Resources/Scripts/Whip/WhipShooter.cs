using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipShooter : MonoBehaviour
{
    [Header("Settings")]
    public GameObject whipPrefab;
    public float baseAttackRate = 5f;
    public float baseDamage = 10f;

    [Header("Level Scaling")]
    public float damageIncreasePerLevel = 2f;
    public float attackRateReductionPerLevel = 0.5f;
    public int maxLevel = 5;

    private float timer;
    private int currentLevel = 1;
    private PlayerController playerController;
    private PlayerDame playerDame;
    public float betweenWhipDelay = 0.2f;
    private bool isAttacking = false;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerDame = GetComponent<PlayerDame>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= GetCurrentAttackRate() && !isAttacking)
        {
            timer = 0f;
            StartCoroutine(PerformWhipAttackSequence());
        }
    }

    public void UpdateWhipLevel(int newLevel)
    {
        currentLevel = Mathf.Clamp(newLevel, 1, maxLevel);
    }

    IEnumerator PerformWhipAttackSequence()
    {
        isAttacking = true;
        Vector2 direction = playerController.GetLastMoveDirection();

        // Chuẩn hóa hướng (chỉ quan tâm hướng ngang)
        direction = direction.x >= 0 ? Vector2.right : Vector2.left;

        // Tấn công chính
        SpawnWhip(direction, GetCurrentDamage());
        yield return new WaitForSeconds(betweenWhipDelay);

        // Tấn công phụ theo level
        if (currentLevel >= 2)
        {
            SpawnWhip(-direction, GetCurrentDamage()); // Hướng ngược lại
        }
        isAttacking = false;
        //if (currentLevel >= 3)
        //{
        //    SpawnWhip(Vector2.up, GetCurrentDamage() * 0.8f); // Hướng lên trên (damage giảm 20%)
        //}
    }

    void SpawnWhip(Vector2 direction, float damage)
    {
        Vector3 spawnPos = transform.position + (Vector3)direction;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject whip = ObjectPooling.Instance.GetFromPool(whipPrefab, spawnPos, rotation);
        WhipAttack whipAttack = whip.GetComponent<WhipAttack>();
        whipAttack.SetDamage(damage);
        whipAttack.Activate();
    }

    float GetCurrentDamage()
    {
        float upgradeDame = playerDame != null ? playerDame.GetCurrentDamage() : 0f;
        return baseDamage + (currentLevel - 1) * damageIncreasePerLevel + upgradeDame;
    }

    float GetCurrentAttackRate()
    {
        return Mathf.Max(0.5f, baseAttackRate - (currentLevel - 1) * attackRateReductionPerLevel);
    }
}
