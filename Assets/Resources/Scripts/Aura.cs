using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour
{
    public float damagePerSecond = 10f;
    public float lifestealPercent = 0.2f;
    private PlayerHealth playerHealth;
    PlayerDame PlayerDame;

    private void Start()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        PlayerDame = GetComponentInParent<PlayerDame>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float critChance = PlayerSkillManager.Instance.critChance;
                float damageThisFrame = (damagePerSecond + PlayerDame.GetCurrentDamage())* Time.deltaTime ;
                damageable.TakeDamage(damageThisFrame);

                if (playerHealth != null)
                {
                    playerHealth.Heal1(damageThisFrame * lifestealPercent);
                }
            }
        }
    }

    public void SetStats(float dps, float lifesteal, float radius)
    {
        damagePerSecond = dps;
        lifestealPercent = lifesteal;
        transform.localScale = new Vector3(radius, radius, 1f);
    }
}
