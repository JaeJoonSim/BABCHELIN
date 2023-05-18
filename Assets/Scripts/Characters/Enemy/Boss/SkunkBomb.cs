using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkunkBomb : BaseMonoBehaviour
{
    public float explosionRadius = 2f;
    public float explosionDamage = 2f;
    public float explosionWaitTime = 5f;

    private Coroutine explosionCoroutine;

    private void Start()
    {
        explosionCoroutine = StartCoroutine(ExplosionBomb());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopCoroutine(explosionCoroutine);
            Explode();
        }
    }

    private IEnumerator ExplosionBomb()
    {
        yield return new WaitForSeconds(explosionWaitTime);
        Explode();
    }

    private void Explode()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in objectsInRange)
        {
            if (col.CompareTag("Player"))
            {
                HealthPlayer player = col.gameObject.GetComponent<HealthPlayer>();
                player.Damaged(gameObject, transform.position, explosionDamage, Health.AttackType.Normal);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
