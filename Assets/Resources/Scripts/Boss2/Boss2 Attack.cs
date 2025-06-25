using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Attack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private Collider2D attackCollider;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Color debugColor = Color.red;

    private void Awake()
    {
        if (attackCollider == null)
        {
            attackCollider = GetComponentInChildren<Collider2D>();
        }
        DisableAttack();
    }

    public void EnableAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
            if (showDebug) Debug.Log("Attack Collider Enabled");

            Invoke(nameof(DisableAttack), attackDuration);
        }
    }

    public void DisableAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
            if (showDebug) Debug.Log("Attack Collider Disabled");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(attackDamage);
                if (showDebug) Debug.Log($"Dealt {attackDamage} damage to player");
            }
        }
    }

}
