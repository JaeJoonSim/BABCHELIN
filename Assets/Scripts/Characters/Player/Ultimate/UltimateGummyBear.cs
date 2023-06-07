using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateGummyBear : MonoBehaviour
{

    private void Start()
    {
        Destroy(gameObject, 6f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.CompareTag("EnemyAttack"))
            Destroy(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("EnemyAttack"))
            Destroy(collision.gameObject);
    }
}
