using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseMonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().Damaged(gameObject, transform.position, damage, Health.AttackType.Normal);
        }
    }
}
