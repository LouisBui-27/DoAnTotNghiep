using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    public float magnetDuration = 5f;
    public float magnetSpeed = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateMagnet(other.transform));
            GetComponent<DropFromBox>()?.NotifyBox();
            Destroy(gameObject);
        }
    }

    IEnumerator ActivateMagnet(Transform player)
    {
        float endTime = Time.time + magnetDuration;
        Dictionary<Transform, float> progressDict = new Dictionary<Transform, float>();

        while (Time.time < endTime)
        {
            var collectibles = ObjectManager.Instance.GetAllCollectibles();

            foreach (Transform item in collectibles)
            {
                if (item != null && item.gameObject.activeInHierarchy)
                {
                    // Khởi tạo progress nếu chưa có
                    if (!progressDict.ContainsKey(item))
                    {
                        progressDict[item] = 0f;
                    }

                    // Tăng progress mỗi frame
                    progressDict[item] += magnetSpeed * Time.deltaTime;

                    // Sử dụng Lerp để di chuyển mượt mà
                    item.position = Vector3.Lerp(
                        item.position,
                        player.position,
                        progressDict[item]
                    );

                    // Thu thập khi đủ gần
                    if (Vector3.Distance(item.position, player.position) < 0.1f)
                    {
                        item.GetComponent<ICollectible>()?.Collect(player);
                        progressDict.Remove(item); // Xóa khỏi dictionary
                    }
                }
            }
            yield return null;
        }
    }
}
