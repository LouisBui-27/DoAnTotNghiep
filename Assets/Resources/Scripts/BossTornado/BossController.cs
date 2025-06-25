using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject tornadoSkill1Prefab;
    public GameObject tornadoSkill2Prefab;
    public Transform bossTransform;
    private Animator animator;
    private Transform player;
    private int skill1Count = 0;
    public float delayBetweenSkills = 3f;


    [SerializeField] private float skill2CastTime = 3f;
    public bool IsCastingSkill2 { get; private set; }
  
    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        StartCoroutine(SkillLoop());
    }

    IEnumerator SkillLoop()
    {
        while (true)
        {
            for (skill1Count = 0; skill1Count < 4; skill1Count++)
            {
                SpawnTornadoSkill1();
                yield return new WaitForSeconds(delayBetweenSkills); 
            }
            yield return new WaitForSeconds(delayBetweenSkills);
            SpawnTornadoSkill2();
            yield return new WaitForSeconds(10);
        }
    }

    void SpawnTornadoSkill1()
    {
        if (player == null) return;

        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
        ObjectPooling.Instance.GetFromPool(tornadoSkill1Prefab, spawnPos, Quaternion.identity);
    }

    void SpawnTornadoSkill2()
    {
        IsCastingSkill2 = true;
       // animator?.SetBool("isAttacking", true);
        var movement = GetComponent<BossMovement>();
        movement?.StopForSkill(skill2CastTime);
        int count = 4;
        for (int i = 0; i < count; i++)
        {
            GameObject tornado = ObjectPooling.Instance.GetFromPool(tornadoSkill2Prefab, transform.position, Quaternion.identity);
            var skill2 = tornado.GetComponent<TornadoSkill2>();
            skill2.angle = i * (360f / count);
            skill2.boss = bossTransform;
        }
        Invoke(nameof(EndSkill2), skill2CastTime);
    }
    void EndSkill2()
    {
        IsCastingSkill2 = false;
        //animator?.SetBool("isAttacking", false);
    }
}
