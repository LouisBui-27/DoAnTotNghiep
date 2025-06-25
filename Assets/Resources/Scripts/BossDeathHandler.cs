using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathHandler : MonoBehaviour
{
    private void Start()
    {
        IBoss boss = GetComponent<IBoss>();
        if (boss != null)
        {
            boss.OnDeath += OnBossDied;
        }
    }

    private void OnDestroy()
    {
        IBoss boss = GetComponent<IBoss>();
        if (boss != null)
        {
            boss.OnDeath -= OnBossDied;
        }
    }

    private void OnBossDied()
    {
        PlayerExp.Instance.ChooseSkillFromBoss();
    }
}
