using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BoxItem : MonoBehaviour,IDamageable
{
    public float maxHP = 50f;
    private float currentHP;
    public float respawnTime = 120f;
    private Vector3 originalPosition;
    private bool isDestroyed = false;
    private int activeDroppedCount = 0;
    [System.Serializable]
    public class DropItem
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float dropChance = 0.5f;
    }

    public List<DropItem> possibleDrops;

    private void Awake()
    {
        originalPosition = transform.position;
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        currentHP -= amount;
        if (currentHP <= 0f)
        {
            isDestroyed = true;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            DropItems();
            if (activeDroppedCount == 0)
            {
                StartCoroutine(RespawnCoroutine());
            }
        }
    }

    void DropItems()
    {
        float totalChance = 0f;
        foreach (var item in possibleDrops) totalChance += item.dropChance;

        float randomPoint = Random.value * totalChance;
        float cumulative = 0f;

        foreach (var item in possibleDrops)
        {
            cumulative += item.dropChance;
            if (randomPoint <= cumulative)
            {
                GameObject drop = ObjectPooling.Instance.GetFromPool(item.prefab, transform.position, Quaternion.identity);
                DropFromBox dropFromBox = drop.GetComponent<DropFromBox>();
                if (dropFromBox != null)
                {
                    dropFromBox.SetSourceBox(this);
                    activeDroppedCount = 1;
                }
                break;
            }
        }
    }

    IEnumerator RespawnCoroutine()
    {
        //// 1. Tắt các thành phần cần thiết thay vì cả GameObject
        //GetComponent<Collider2D>().enabled = false;
        //GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // 2. Reset trạng thái
        currentHP = maxHP;
        isDestroyed = false;
        transform.position = originalPosition;

        // 3. Bật lại các thành phần
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }
    public void NotifyCollected()
    {
        activeDroppedCount--;
        if (activeDroppedCount <= 0)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }
}
