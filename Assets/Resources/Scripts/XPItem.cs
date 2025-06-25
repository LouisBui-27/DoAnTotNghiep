using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPItem : MonoBehaviour,ICollectible
{
    public int xpAmount = 10;
    private bool isCollected = false;

    private void OnEnable()
    {
        ObjectManager.Instance?.Register(transform);
        isCollected = false; // Reset để object có thể dùng lại
    }
    private void OnDisable() => ObjectManager.Instance?.Unregister(transform);

    public void Collect(Transform player)
    {
        if (isCollected) return;

        isCollected = true;
       
        player.GetComponent<PlayerExp>().gainXP(xpAmount);
        Debug.Log($"Collected EXP: {xpAmount} at {Time.time} seconds");
        StartCoroutine(ReturnToPoolWithDelay(0f));
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
            AudioManager.Instance.PlayPlayerEXP();
            Collect(collision.transform);
        }
    }
}
