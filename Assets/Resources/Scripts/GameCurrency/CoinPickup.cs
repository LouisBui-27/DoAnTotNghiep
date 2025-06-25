using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private int coinAmount = 10;
    private bool isCollected = false;

    private void OnEnable() => ObjectManager.Instance?.Register(transform);
    private void OnDisable() => ObjectManager.Instance?.Unregister(transform);
  
    public void Collect(Transform player)
    {
        if (isCollected) return;

        isCollected = true;
        CurrencyManage.Instance.AddSessionMoney(coinAmount);
        GetComponent<DropFromBox>()?.NotifyBox();
        StartCoroutine(ReturnToPoolWithDelay(0.1f));
    }

    private IEnumerator ReturnToPoolWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect(collision.transform);
            AudioManager.Instance.PlayPlayerCoin();
        }
    }
}
