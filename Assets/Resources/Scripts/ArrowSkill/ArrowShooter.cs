using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float shootInterval = 1.5f;

    private PlayerController playerController;
    private PlayerDame PlayerDame;

    private int arrowDamage = 10; // Mặc định damage của mũi tên
    private float arrowSpeed = 10f; // Mặc định speed của mũi tên

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        PlayerDame = GetComponent<PlayerDame>();
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            ShootArrow();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || playerController == null) return;

        Vector2 shootDirection = playerController.GetLastMoveDirection().normalized;
        AudioManager.Instance.PlayPlayerArrow();

        // Lấy số mũi tên theo kỹ năng
        int arrowCount = 1 + PlayerSkillManager.Instance.extraBullets;

        float spreadAngle = 10f; // Góc lệch giữa các mũi tên
        float startAngle = -(arrowCount - 1) * spreadAngle / 2f;

        for (int i = 0; i < arrowCount; i++)
        {
            float angle = startAngle + i * spreadAngle;
            Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * shootDirection;

            GameObject arrow = ObjectPooling.Instance.GetFromPool(arrowPrefab, transform.position, Quaternion.identity);
            //float arrowAngle = Mathf.Atan2(rotatedDirection.y, rotatedDirection.x) * Mathf.Rad2Deg;
            //arrow.transform.rotation = Quaternion.Euler(0f, 0f, arrowAngle);
            Arrow arrowScript = arrow.GetComponent<Arrow>();
            float finalDame = arrowDamage + PlayerDame.GetCurrentDamage();
            if (arrowScript != null)
            {
                arrowScript.SetDamage(finalDame);
                arrowScript.SetSpeed(arrowSpeed);
                arrowScript.SetDirection(rotatedDirection.normalized);
                arrowScript.Activate();
            }
        }
    }
    public void UpdateArrowStats(int newDamage, float newSpeed)
    {
        arrowDamage = newDamage;
        arrowSpeed = newSpeed;
    }
}
