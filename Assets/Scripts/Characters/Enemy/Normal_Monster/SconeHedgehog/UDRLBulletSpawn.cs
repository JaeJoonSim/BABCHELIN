using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDRLBulletSpawn : MonoBehaviour
{
    public GameObject[] Bullet;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Bullet[0], transform.position + Vector3.up, Quaternion.identity);
        Instantiate(Bullet[1], transform.position + Vector3.right, Quaternion.identity);
        Instantiate(Bullet[2], transform.position + Vector3.down, Quaternion.identity);
        Instantiate(Bullet[3], transform.position + Vector3.left, Quaternion.identity);
    }

    private void Update()
    {
        Destroy(gameObject, 2f);
    }
}
