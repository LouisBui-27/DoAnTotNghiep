using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSkillBase : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] protected Animator animator;
    protected bool isImpacted = false;

    protected virtual void OnEnable()
    {
        isImpacted = false;
        if (animator != null)
        {
            animator.Play("Idle1", 0, 0f); // Reset về đầu animation idle
        }
    }

    protected void HandleImpact()
    {
        if (isImpacted) return;
        isImpacted = true;

        if (animator != null)
        {
            animator.SetTrigger("onImpact");
        }
        else
        {
            ReturnToPoolImmediately();
        }
    }

    protected void ReturnToPoolImmediately()
    {
        ObjectPooling.Instance?.ReturnToPool(gameObject);
    }

    // Gọi từ Animation Event
    public void OnImpactAnimationComplete()
    {
        ReturnToPoolImmediately();
    }
}
