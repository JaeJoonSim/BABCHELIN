using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateBbebbero : MonoBehaviour
{
    public float Damage;
    public float destructionGauge;

    UltimateRotation ultimateRotation;

    public ParticleSystem HitEffet;
    void Start()
    {
        ultimateRotation = GetComponentInChildren<UltimateRotation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ultimateRotation.RotateObj())
        {
            HitEffet.Play();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Skunk skunk = collision.GetComponent<Skunk>();
            Vector3 collisionPoint = collision.ClosestPoint(transform.position);
            if (skunk != null)
                collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal, skunk.destructionCount);
            else
                collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal);

            PartDestructionGauge(collision, destructionGauge);
            Destroy(gameObject);
        }
        else if (collision.tag == "DestroyableObject ")
        {
            Vector3 collisionPoint = collision.ClosestPoint(transform.position);
            Destroy(collision.gameObject);
        }
    }

    protected virtual void PartDestructionGauge(Collider2D other, float gauge)
    {
        Skunk boss = other.GetComponent<Skunk>();
        if (boss != null)
        {
            boss.DestructionGauge -= gauge;
        }
    }
}
