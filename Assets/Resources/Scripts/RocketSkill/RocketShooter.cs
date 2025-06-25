using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShooter : MonoBehaviour
{
    public GameObject rocketPrefab;
    public float shootInterval = 3f;
    public float rocketDamage = 30;
    public float rocketSpeed = 5f;

    private PlayerController playerController;
    private PlayerDame playerDame;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerDame = GetComponent<PlayerDame>();
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        while (true)
        {
            ShootRocket();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void ShootRocket()
    {
        if (rocketPrefab == null || playerController == null) return;

        Vector2 direction = playerController.GetLastMoveDirection().normalized;
        AudioManager.Instance.PlayPlayerRocket();

        GameObject rocket = ObjectPooling.Instance.GetFromPool(rocketPrefab, transform.position, Quaternion.identity);
        RocketBullet rocketScript = rocket.GetComponent<RocketBullet>();
        float finalDame = rocketDamage + playerDame.GetCurrentDamage();
        if (rocketScript != null)
        {
            rocketScript.SetDirection(direction);
            rocketScript.SetDamage(finalDame);
            rocketScript.SetSpeed(rocketSpeed);
            rocketScript.SetPlayerBaseDamage(playerDame.GetCurrentDamage());
            rocketScript.Activate();
        }
    }
    public void UpdateRocketStats(float newDamage)
    {
        rocketDamage = newDamage;
    }
}
