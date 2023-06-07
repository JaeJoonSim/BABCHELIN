using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterBulletSpawn : MonoBehaviour
{
    public GameObject[] Bullet;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Bullet[0], transform.position + Vector3.right, Quaternion.identity);
        Instantiate(Bullet[1], transform.position + Vector3.left, Quaternion.identity);
        Bullet[2].transform.position = new Vector3(transform.position.x - 0.6f, transform.position.y + 0.6f, transform.position.z);
        Instantiate(Bullet[2]);
        Bullet[3].transform.position = new Vector3(transform.position.x + 0.6f, transform.position.y + 0.6f, transform.position.z);
        Instantiate(Bullet[3]);
        Bullet[4].transform.position = new Vector3(transform.position.x - 0.6f, transform.position.y - 0.6f, transform.position.z);
        Instantiate(Bullet[4]);
        Bullet[5].transform.position = new Vector3(transform.position.x + 0.6f, transform.position.y - 0.6f, transform.position.z);
        Instantiate(Bullet[5]);
    }

    private void Update()
    {
        Destroy(gameObject, 2f);
    }
}
