using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdRoomSpawnMonster : BaseMonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public BoxCollider2D spawnArea;
    public int numberOfMonsters;
    private int activeMonsters;

    [Space]
    public GameObject lastRoomEnter;

    private void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < numberOfMonsters - 1; i++)
            {
                SpawnMonster();
            }
        }
        spawnArea.enabled = false;
    }

    private void SpawnMonster()
    {
        GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length - 1)];

        float x = Random.Range(-spawnArea.size.x / 2, spawnArea.size.x / 2);
        float y = Random.Range(-spawnArea.size.y / 2, spawnArea.size.y / 2);
        Vector2 localSpawnPosition = new Vector2(x, y);
        Vector2 worldSpawnPosition = spawnArea.transform.TransformPoint(localSpawnPosition);

        GameObject spawnedMonster = Instantiate(monsterPrefab, worldSpawnPosition, Quaternion.identity);
        activeMonsters++;

        Health monsterHealth = spawnedMonster.GetComponent<Health>();
        if (monsterHealth != null)
        {
            monsterHealth.OnDie += HandleMonsterDeath;
        }
    }


    private void HandleMonsterDeath()
    {
        activeMonsters--;
        if (activeMonsters <= 0)
        {
            lastRoomEnter.SetActive(true);
        }
    }
}
