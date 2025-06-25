using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossController : EnemyController,IBoss
{
    //public new event System.Action OnDeath;
    protected override void HandleDropExp()
    {
        // Không làm gì cả — boss không rơi exp
    }
    protected override void Die()
    { 
        base.Die();
      //  OnDeath?.Invoke();
       // ObjectPooling.Instance.ReturnToPool(gameObject);
    }
}
