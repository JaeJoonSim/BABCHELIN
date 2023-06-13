using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPot : UnitObject
{
    [SerializeField] float spawnCycleTime;
    private float time;
    [SerializeField] float spawnRange;
    [SerializeField] GameObject[] mosterObject;
    private GameObject parentObj;

    public GameObject spawnEffect;
    public GameObject BrokenEffect;

    [Space]
    [SerializeField] private int spawnMax;


    private void Start()
    {
        if(transform.parent.gameObject != null)
            parentObj = transform.parent.gameObject;
    }

    public override void Update()
    {
        time += Time.deltaTime;

        if(time >= spawnCycleTime && parentObj.transform.childCount < spawnMax + 1)
        {
            SpawnMonster();
            time = 0;
        }

        if(health.CurrentHP() <= 0)
        {
            GameObject brokenEffect = Instantiate(BrokenEffect);
            brokenEffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            Destroy(gameObject);
        }
    }

    public void SpawnMonster()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRange;
        Vector3 dropPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y + randomPoint.y, transform.position.z);

        //GameObject spawnMonster = mosterObject[Random.Range(0, mosterObject.Length)];
        GameObject spawneffect = Instantiate(spawnEffect);
        spawneffect.transform.position = new Vector3(dropPosition.x, dropPosition.y, dropPosition.z - 0.5f);

        GameObject spawnMonster = Instantiate(mosterObject[Random.Range(0, mosterObject.Length)], transform.parent);
        //Instantiate(spawnMonster, dropPosition, Quaternion.identity);
        spawnMonster.transform.position = dropPosition;
        //spawnMonster.transform.SetParent(transform.parent);
    }
}