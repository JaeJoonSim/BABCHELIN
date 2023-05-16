using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPot : UnitObject
{
    [SerializeField] float spawnCycleTime;
    private float time;
    [SerializeField] float spawnRange;
    [SerializeField] GameObject[] mosterObject;

    private void Update()
    {
        time += Time.deltaTime;

        if(time >= spawnCycleTime)
        {
            SpawnMonster();
            time = 0;
        }

        if(health.CurrentHP() <= 0)
        {
            Destroy(gameObject, 1f);
        }
    }

    public void SpawnMonster()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRange;
        Vector3 dropPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y + randomPoint.y, transform.position.z);

        GameObject item = mosterObject[Random.Range(0, mosterObject.Length)];
        Instantiate(item, dropPosition, Quaternion.identity);
    }
}
