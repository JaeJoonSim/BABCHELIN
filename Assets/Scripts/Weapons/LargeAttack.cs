using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeAttack : MonoBehaviour
{
    public float speed;
    public float range;
    Vector2 spownPos;

    // Update is called once per frame
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
        Debug.Log(collision.name);
        if (collision.tag == "Enemy")
            Destroy(gameObject);
    }
}
