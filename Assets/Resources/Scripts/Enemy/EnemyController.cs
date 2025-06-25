using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour, IDamageable
{
    public float speed = 3f;
    public float hp;
    public float hpMax;
    public float damage = 3f;
    public Rigidbody2D rb;
    private AIPath aiPath;

    [SerializeField] Transform Target;
    [SerializeField] GameObject xpPrefab;

    private ObjectPooling objectPooling;
    public event Action OnDeath;

    private void OnEnable()
    {
        EnemyManage.instance?.Register(transform);
    }

    private void OnDisable()
    {
        EnemyManage.instance?.Unregister(transform);
        OnDeath = null;
    }
    private void Awake()
    {
        aiPath = GetComponent<AIPath>();
        if (aiPath == null)
        {
            Debug.LogError("AIPath component không tìm thấy trên Enemy!");
            enabled = false;
            return;
        }
        aiPath.maxSpeed = speed; // Gán tốc độ từ biến speed của bạn
    }
    //private void Start()
    //{
    //    Target = GameObject.FindGameObjectWithTag("Player").transform;
    //    //objectPooling = FindObjectOfType<ObjectPooling>();
    //}
    private void Update()
    {
        if (aiPath != null && aiPath.enabled)
        {
            Vector3 scale = transform.localScale;

            if (aiPath.desiredVelocity.x > 0.1f)
            {
                scale.x = Mathf.Abs(scale.x); // Hướng phải
                transform.localScale = scale;
            }
            else if (aiPath.desiredVelocity.x < -0.1f)
            {
                scale.x = -Mathf.Abs(scale.x); // Hướng trái
                transform.localScale = scale;
            }
        }
    }
    public void SetTarget(Transform newTarget)
    {
        Target = newTarget;
        var setter = GetComponent<AIDestinationSetter>();
        if (setter != null)
        {
            setter.target = newTarget;
        }
    }
    public void Initialize(int waveIndex, float baseHp, float hpScaleFactorPerWave)
    {
        hpMax = baseHp + (waveIndex * hpScaleFactorPerWave); // Tính toán HP Max dựa trên baseHp và scaling
        hp = hpMax; // Gán HP hiện tại bằng HP Max đã tính
        gameObject.SetActive(true);
       // Debug.Log($"Enemy {gameObject.name} Initialized for Wave {waveIndex}: HP = {hpMax}");
    }

    public void TakeDamage(float damage)
    {
      //  Debug.Log("nhan " + damage);
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
        //Debug.Log("Enemy Health: " + hp);
    }
    protected virtual void Die()
    {
        // AudioManager.Instance.PlayEnemyDie();
        if (OnDeath != null) // Kiểm tra xem có subscriber nào không
        {
            OnDeath.Invoke(); // <--- KÍCH HOẠT EVENT Ở ĐÂY
        }
        HandleDropExp();
        ReturnToPool();
        UpdateMission();

    }
    protected virtual void HandleDropExp()
    {
        if (xpPrefab != null)
        {
            ObjectPooling.Instance.GetFromPool(xpPrefab, transform.position, Quaternion.identity);
        }
    }

    protected virtual void ReturnToPool()
    {
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }

    protected virtual void UpdateMission()
    {
        var missionMgr = MissionManager.Instance;
        if (missionMgr != null)
        {
            missionMgr.UpdateMissionProgress("mission_1", missionMgr.allMissions.Find(m => m.id == "mission_1").currentProgress + 1);
            missionMgr.UpdateMissionProgress("mission_6", missionMgr.allMissions.Find(m => m.id == "mission_6").currentProgress + 1);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IDamageable target = other.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage); // dùng biến 'damage' đã có sẵn
            }
        }
        
    }
    public void SetExpPrefab(GameObject prefab)
    {
        xpPrefab = prefab;
    }
    //public void ScaleHealthByWave(int waveIndex, float healthPerWave = 10f)
    //{
    //    hpMax = hpMax + waveIndex * healthPerWave;
    //}
}
