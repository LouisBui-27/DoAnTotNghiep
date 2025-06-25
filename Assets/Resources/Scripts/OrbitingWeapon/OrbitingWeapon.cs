using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingWeapon : MonoBehaviour
{
    public GameObject orbitingPrefab;

    private List<GameObject> currentObjects = new List<GameObject>();
    private float rotateSpeed = 180f;

    private int objectCount;
    private float damage;
    private float appearTime;
    private float disappearTime;
   
    // Next stats (sẽ dùng ở vòng tiếp theo)
    private int nextObjectCount;
    private float nextDamage;
    private float nextAppearTime;
    private float nextDisappearTime;
    PlayerDame playerDame;
    private Coroutine cycleCoroutine;

    private void Awake()
    {
        playerDame = GetComponent<PlayerDame>();
    }
    public void UpdateSkill(int count, float dmg, float appear, float disappear)
    {
        nextObjectCount = count;
        nextDamage = dmg;
        nextAppearTime = appear;
        nextDisappearTime = disappear;

        // Nếu chưa từng chạy vòng xoay thì khởi động
        if (cycleCoroutine == null)
            cycleCoroutine = StartCoroutine(SkillCycle());
    }

    IEnumerator SkillCycle()
    {
        objectCount = nextObjectCount;
        damage = nextDamage;
        appearTime = nextAppearTime;
        disappearTime = nextDisappearTime;

        while (true)
        {
            SpawnOrbitingObjects();
            yield return new WaitForSeconds(appearTime);
            ClearOrbitingObjects();
            yield return new WaitForSeconds(disappearTime);

            // Cập nhật thông số từ lần gọi UpdateSkill gần nhất
            objectCount = nextObjectCount;
            damage = nextDamage;
            appearTime = nextAppearTime;
            disappearTime = nextDisappearTime;
        }
    }

    void SpawnOrbitingObjects()
    {
        float radius = 1.5f;
        float angleStep = 360f / objectCount;
        float extraDame = playerDame != null ? playerDame.GetCurrentDamage() : 0f;
        float finalDamage = damage + extraDame;

        for (int i = 0; i < objectCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            Vector3 spawnPosition = transform.position + offset;

            GameObject obj = ObjectPooling.Instance.GetFromPool(
                orbitingPrefab,
                spawnPosition,
                Quaternion.identity
            );

            obj.transform.SetParent(transform);
            OrbitingObject orbit = obj.GetComponent<OrbitingObject>();
            orbit.Initialize(transform, angle, radius, rotateSpeed, finalDamage);

            currentObjects.Add(obj);
        }
    }

    void ClearOrbitingObjects()
    {
        foreach (var obj in currentObjects)
        {
            if (obj != null) ObjectPooling.Instance.ReturnToPool(obj);
        }
        currentObjects.Clear();
    }
}
