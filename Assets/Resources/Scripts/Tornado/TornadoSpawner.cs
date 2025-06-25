using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TornadoSpawner : MonoBehaviour
{
    public GameObject tornadoPrefab;
    public float spawnInterval = 3f;
   // private float timer = 0f;

    private float tornadoDamage = 15f;
    private float tornadoSpeed = 3f;
    private int tornadoCount = 1;
    PlayerDame playerDame;
    private void Awake()
    {
        playerDame = GetComponent<PlayerDame>();
    }
    private void Start()
    {
        
        StartCoroutine(spawnTornado());
    }
    IEnumerator spawnTornado()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            SpawnTornados();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void UpdateTornadoStats(float damage, float speed, int count, float spawn)
    {
        tornadoDamage = damage;
        tornadoSpeed = speed;
        tornadoCount = count;
        spawnInterval = spawn;
    }

    void SpawnTornados()
    {
        float extraDame = playerDame != null ? playerDame.GetCurrentDamage() : 0f;
        float finalDamage = tornadoDamage + extraDame;
        for (int i = 0; i < tornadoCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            GameObject tornado = ObjectPooling.Instance.GetFromPool(tornadoPrefab, transform.position + offset, Quaternion.identity);
            Tornado tornadoScript = tornado.GetComponent<Tornado>();
            if (tornadoScript != null)
            {
                tornadoScript.damage = finalDamage;
                tornadoScript.moveSpeed = tornadoSpeed;
            }
        }
        AudioManager.Instance.PlayPlayerTornado();
    }
}
