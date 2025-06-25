using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float damage = 10f;
    public float duration = 5f;
    public float directionChangeInterval = 0.5f;
    //private Vector2 direction;

    private Vector2 currentDirection;
    private Coroutine returnCoroutine;
    private Coroutine changeDirectionCoroutine;

    void OnEnable()
    {
        ChooseRandomDirection();
        returnCoroutine = StartCoroutine(ReturnToPoolAfterTime(duration));
        changeDirectionCoroutine = StartCoroutine(ChangeDirectionRoutine());
    }

    void OnDisable()
    {
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        if (changeDirectionCoroutine != null) StopCoroutine(changeDirectionCoroutine);
    }

    void Update()
    {
        transform.Translate(currentDirection * moveSpeed * Time.deltaTime);
    }

    private IEnumerator ReturnToPoolAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(directionChangeInterval);
            ChooseRandomDirection();
        }
    }

    private void ChooseRandomDirection()
    {
        currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
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
            }
        }
    }

}
