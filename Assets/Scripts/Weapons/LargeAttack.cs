using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeAttack : MonoBehaviour
{
    public float speed;
    public float range;
    Vector2 spownPos;
    public ParticleSystem HitEffet;

    private void Start()
    {
        spownPos = transform.position;
    }
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, spownPos) > range)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            
            collision.GetComponent<Health>().Damaged(gameObject, transform.position, 10f);
            Instantiate(HitEffet, transform.position, Quaternion.identity);
            //Instantiate(HitEffet, collision.ClosestPoint(transform.position), Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
