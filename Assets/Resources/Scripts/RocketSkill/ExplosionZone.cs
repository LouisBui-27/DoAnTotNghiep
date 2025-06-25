using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionZone : MonoBehaviour
{
    public float explosionDame = 5f;
    public float duration = 2f;
    private float playerBaseDamageFromShooter = 0f;

    private void OnEnable()
    {
        Invoke("Deactivate", duration);
     
        
    }

    private void OnDisable()
    {
        CancelInvoke("Deactivate");
        playerBaseDamageFromShooter = 0f;
    }
    public void SetPlayerBaseDamage(float baseDmg)
    {
        playerBaseDamageFromShooter = baseDmg;
    }
    public void SetDamage(float damage)
    {
        explosionDame = damage;
    }
    public void SetExplosionScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1f);
    }
    void Deactivate()
    {
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable enemy = other.GetComponent<IDamageable>();
            if (enemy != null)
            {
                float critChance = PlayerSkillManager.Instance.critChance;
                float finalDamage = explosionDame+playerBaseDamageFromShooter;


                if (Random.value < critChance)
                {
                    finalDamage *= 2f; // x2 damage cho chí mạng
                    Debug.Log("💥 Chí mạng! Gây " + finalDamage + " sát thương.");
                }
                enemy.TakeDamage(finalDamage * Time.deltaTime);
            }
        }
    }
    public void PlayExplosionSound()
    {
 AudioManager.Instance.PlayPlayerBom();
    }
   
}
