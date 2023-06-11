using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterAttackPattern1 : MonoBehaviour
{
    public GameObject[] Bullet;
    private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Bullet[0], transform.position + Vector3.right, Quaternion.identity);
        Instantiate(Bullet[1], transform.position + Vector3.left, Quaternion.identity);
        spawnPoint = new Vector3(transform.position.x - 0.6f, transform.position.y + 0.6f, transform.position.z);
        Instantiate(Bullet[2], spawnPoint, Quaternion.identity);
        spawnPoint = new Vector3(transform.position.x + 0.6f, transform.position.y + 0.6f, transform.position.z);
        Instantiate(Bullet[3], spawnPoint, Quaternion.identity);
        spawnPoint = new Vector3(transform.position.x - 0.6f, transform.position.y - 0.6f, transform.position.z);
        Instantiate(Bullet[4], spawnPoint, Quaternion.identity);
        spawnPoint = new Vector3(transform.position.x + 0.6f, transform.position.y - 0.6f, transform.position.z);
        Instantiate(Bullet[5], spawnPoint, Quaternion.identity);
    }

    private void Update()
    {
        Destroy(gameObject, 2f);
    }
}
