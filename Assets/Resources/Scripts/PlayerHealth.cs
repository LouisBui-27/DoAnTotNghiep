using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public float baseHealth = 100f;
    public float maxHP;

    private float currentHP;

    private void Start()
    {
        maxHP = baseHealth;
        currentHP = maxHP;
        HealthUI.instance?.UpdateHealthUI(currentHP, maxHP);
    }
    public void setUpgradeMaxHealth(float newMax)
    {
        maxHP += newMax;

        currentHP = Mathf.Max(currentHP, maxHP);
        HealthUI.instance?.UpdateHealthUI(currentHP, maxHP);
        Debug.Log("Max HP mới: " + maxHP);
    }
    public void setMaxHealth(float newMax)
    {
        maxHP = newMax;

        currentHP = Mathf.Min(currentHP, maxHP);
        HealthUI.instance?.UpdateHealthUI(currentHP, maxHP);
        Debug.Log("Max HP mới: " + maxHP);
    }
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Đảm bảo không dưới 0
        HealthUI.instance?.UpdateHealthUI(currentHP, maxHP);
        AudioManager.Instance.PlayPlayerTakeDam();
        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void Heal(float healAmount)
    {
        currentHP += healAmount;
        currentHP = Mathf.Min(currentHP, maxHP);
        HealthUI.instance?.UpdateHealthUI(currentHP,maxHP);
        AudioManager.Instance.PlayPlayerHeal();
    }
    public void Heal1(float healAmount)
    {
        currentHP += healAmount;
        currentHP = Mathf.Min(currentHP, maxHP);
        HealthUI.instance?.UpdateHealthUI(currentHP, maxHP);
        
    }
    void Die()
    {
        Debug.Log("Player died!");
        // Xử lý khi chết (tắt nhân vật, load lại game, v.v.)
      //  CurrencyManage.Instance.CommitSessionMoney();
        AudioManager.Instance.PlayPlayerDie();
        GameOverUI.instance?.ShowResult(false);
    }
}
