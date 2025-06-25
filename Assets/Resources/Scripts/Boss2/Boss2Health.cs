using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Health : MonoBehaviour, IDamageable,IBoss
{
    public float maxHP = 100f;
    private float currentHP;
   
    [Header("References")]
    [SerializeField] private Boss2Controller boss2Controller;
    [SerializeField] private Animator animator;

    public event Action OnDeath;

    // [SerializeField] private HealthBar healthBar;

    private void OnEnable()
    {
        EnemyManage.instance?.Register(transform);
    }
    private void OnDisable()
    {
        EnemyManage.instance?.Unregister(transform);
        OnDeath = null;
    }

    private void Awake()
    {
        currentHP = maxHP;
        boss2Controller = GetComponent<Boss2Controller>();
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
       // if (animator != null) animator.SetTrigger("Hit");
        if (boss2Controller != null)
        {
            boss2Controller.CheckPhase(currentHP / maxHP);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (OnDeath != null) // Kiểm tra xem có subscriber nào không
        {
            OnDeath.Invoke(); // <--- KÍCH HOẠT EVENT Ở ĐÂY
        }
        Debug.Log("Boss2 died!");
        boss2Controller.moveSpeed = 0f;
        ObjectPooling.Instance.ReturnToPool(gameObject);

    }
    IEnumerator DisableAfterAnimation()
    {
        yield return new WaitForSeconds(.5f);
        gameObject.SetActive(false);
    }
}
