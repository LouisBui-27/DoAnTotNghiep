using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float speed = 7f;
    public float maxDistance = 5f;
    public float returnSpeed = 10f;
    public float damage = 20f;

    private Transform player;
    private Vector3 startPosition;
    private bool isReturning = false;
    private Vector3 forwardTarget;

    public void Init(Transform playerTransform, Transform enemyTarget, float moveSpeed, float returnSpd, float maxDis)
    {
        StopAllCoroutines();
        isReturning = false; // Reset trạng thái bay
        player = playerTransform;
        startPosition = transform.position;
        speed = moveSpeed;
        returnSpeed = returnSpd;
        maxDistance = maxDis;


        if (enemyTarget != null)
        {
            Vector3 dir = (enemyTarget.position - transform.position).normalized;
            forwardTarget = transform.position + dir * maxDistance;
        }
        else
        {
            Vector3 dir = (playerTransform.localScale.x >= 0) ? playerTransform.right : -playerTransform.right;
            forwardTarget = transform.position + dir.normalized * maxDistance;
        }

        StartCoroutine(FlyRoutine());
    }
    IEnumerator FlyRoutine()
    {
        // Bay tới mục tiêu (hoặc khoảng cách tối đa)
        while (!isReturning && Vector3.Distance(transform.position, forwardTarget) >0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, forwardTarget, speed * Time.deltaTime);
            yield return null;
        }

        // Quay về phía sau player
        isReturning = true;
        Vector3 backPosition = player.position - player.right * 1.5f;

        while (Vector3.Distance(transform.position, backPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, backPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }

        ObjectPooling.Instance.ReturnToPool(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            //var enemy = other.GetComponent<EnemyController>();
            IDamageable enemy = other.GetComponent<IDamageable>(); 
            if (enemy != null)
            {
                float critChance = PlayerSkillManager.Instance.critChance;
                float finalDamage = damage;

                if (Random.value < critChance)
                {
                    finalDamage *= 2f; // x2 damage cho chí mạng
                    Debug.Log("💥 Chí mạng! Gây " + finalDamage + " sát thương.");
                }

                enemy.TakeDamage(finalDamage);
            }
        }
    }
}
