using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSkill2 : TornadoSkillBase
{
    public Transform boss;
    public float angleSpeed = 90f;
    public float expandSpeed = 1f;
    public float duration = 7f;
    public float damage = 8f;

    public float angle;
    private float radius = 0f;
    private float timer = 0f;

    //[Header("Visual Effects")]
    //[SerializeField] private ParticleSystem chargeParticles;
    //[SerializeField] private float chargeTime = 0.5f;

    protected override void OnEnable()
    {
        timer = 0f;
        radius = 0f;

        // Hiệu ứng tích tụ trước khi bắn
        //if (chargeParticles != null)
        //{
        //    chargeParticles.Play();
        //}

        StartCoroutine(StartAfterCharge());
    }

    private IEnumerator StartAfterCharge()
    {
        float currentAngle = angle;
       // float currentRadius = 0f;

        // Giai đoạn tích tụ
        //while (timer < chargeTime)
        //{
        //    timer += Time.deltaTime;
        //    float progress = timer / chargeTime;

        //    // Di chuyển từ từ ra vị trí ban đầu
        //    currentRadius = Mathf.Lerp(0f, 1f, progress) * radius;
        //    UpdatePosition(currentAngle, currentRadius);
        //    yield return null;
        //}

        // Bắt đầu di chuyển bình thường
        while (timer < duration)
        {
            timer += Time.deltaTime;
            angle += angleSpeed * Time.deltaTime;
            radius += expandSpeed * Time.deltaTime;
            UpdatePosition(angle, radius);
            yield return null;
        }

        ObjectPooling.Instance.ReturnToPool(gameObject);
    }

    private void UpdatePosition(float currentAngle, float currentRadius)
    {
        if (boss == null) return;

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * currentRadius;
        transform.position = boss.position + (Vector3)offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isImpacted) return;

        // Xử lý damage
        IDamageable target = other.GetComponent<IDamageable>();
        target?.TakeDamage(damage);

        HandleImpact();
    }
}
