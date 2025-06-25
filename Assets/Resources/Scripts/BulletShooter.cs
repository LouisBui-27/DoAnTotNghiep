using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public GameObject bulletPrefab; 
  //  public Transform firePoint;     
    public float attackRate = 1f;   
    public float bulletSpeed = 7f;
    public float bulletDamage = 10f;
   
    // private Vector2 lastMoveDirection = Vector2.right;

    private PlayerController playerController;
    private PlayerDame PlayerDame;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        PlayerDame = GetComponent<PlayerDame>();    
        //  InvokeRepeating("AutoAttack", 0f, attackRate);
        StartCoroutine(ShootRoutine());
    }

    //void Update()
    //{
        
    //    Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //    //if (moveInput != Vector2.zero)
    //    //{
    //    //    lastMoveDirection = moveInput.normalized;
    //    //}
    //}

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(FireBulletsInLine());
            yield return new WaitForSeconds(attackRate);
        }
    }
    IEnumerator FireBulletsInLine()
    {
        Vector2 direction = playerController.GetLastMoveDirection().normalized;
        int bulletCount = 1 + PlayerSkillManager.Instance.extraBullets;

        
        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = ObjectPooling.Instance.GetFromPool(bulletPrefab,transform.position,Quaternion.identity);
            //bullet.transform.position = firePoint.position;
            //bullet.transform.rotation = Quaternion.identity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            float finaldame = bulletDamage + PlayerDame.GetCurrentDamage();
            if (bulletScript != null)
            {

                bulletScript.SetDamage(finaldame);
                bulletScript.SetSpeed(bulletSpeed);
                bulletScript.SetDirection(direction);
                bulletScript.Activate();
            }

            AudioManager.Instance.PlayPlayerShoot();
            yield return new WaitForSeconds(0.2f); // Độ trễ giữa các viên đạn
        }
    }
    public void UpdateArrowStats(int newDamage, float newSpeed)
    {
        bulletDamage = newDamage;
        bulletSpeed = newSpeed;
    }
}
