using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;
    private HashSet<Transform> collectibles = new HashSet<Transform>();

    private void Awake() => Instance = this;

    public void Register(Transform item) => collectibles.Add(item);
    public void Unregister(Transform item) => collectibles.Remove(item);

    public List<Transform> GetAllCollectibles()
    {
        var validCollectibles = new List<Transform>();
        foreach (var item in collectibles)
        {
            if (item != null && item.gameObject.activeInHierarchy)
                validCollectibles.Add(item);
        }
        return validCollectibles;
    }
}
