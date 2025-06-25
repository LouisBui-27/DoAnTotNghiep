using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public float basePickupRadius = 0f;
    private float currentPickupRadius;
    private float currentUpdateRadius = 0f;

    void Start()
    {
        currentPickupRadius = basePickupRadius;
    }

    public void UpdatePickupRadius(float newRadius)
    {
        currentPickupRadius = newRadius;
    }
    public void increaseUpgradeRadius(float amount)
    {
        currentUpdateRadius += amount;
    }
    void Update()
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, currentPickupRadius + currentUpdateRadius);
        foreach (Collider2D item in items)
        {
            if (item.CompareTag("Object"))
            {
                // hút item
                item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, 10f * Time.deltaTime);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Application.isPlaying ? currentPickupRadius + currentUpdateRadius : basePickupRadius);
    }
}
