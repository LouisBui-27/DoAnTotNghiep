using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipAttack : MonoBehaviour
{
    private float damage;
    public float lifetime = 0.3f;
    private Coroutine disableCoroutine;
    //  public ParticleSystem hitEffect;

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    IEnumerator DisableAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }

    public void Activate()
    {
        // Hủy coroutine cũ nếu có
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        disableCoroutine = StartCoroutine(DisableAfterLifetime());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {

                float critChance = PlayerSkillManager.Instance.critChance;
                float finalDamage = damage;

                if (Random.value < critChance)
                {
                    finalDamage *= 2f; // x2 damage cho chí mạng
                    Debug.Log("💥 Chí mạng! Gây " + finalDamage + " sát thương.");
                }

                damageable.TakeDamage(finalDamage);
                //PlayHitEffect(other.transform.position);
            }
        }
    }
    private void OnDisable()
    {
        // Đảm bảo hủy coroutine khi object bị tắt
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
    }

    //void PlayHitEffect(Vector3 position)
    //{
    //    if (hitEffect != null)
    //    {
    //        Instantiate(hitEffect, position, Quaternion.identity);
    //    }
    //}
}
