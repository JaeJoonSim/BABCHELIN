using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateBbebbero : MonoBehaviour
{
    public float Damage;
    public float destructionGauge;

    UltimateRotation ultimateRotation;

    public bool isDamaged;

    public ParticleSystem HitEffet;
    void Start()
    {
        ultimateRotation = GetComponentInChildren<UltimateRotation>();
    }

    void Update()
    {
        if (ultimateRotation.RotateObj() && !isDamaged)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            HitEffet.Play();
            Destroy(gameObject, 2f);
            isDamaged = true;
        }
    }

    public void getStatus(UltimateStatus val)
    {
        Damage = val.Damage;
        destructionGauge = val.destructionGauge;
        transform.localScale = Vector3.one * val.size;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Enemy")
        {
            Skunk skunk = collision.GetComponent<Skunk>();
            Vector3 collisionPoint = collision.ClosestPoint(transform.position);
            if (skunk != null)
            {
                collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal, skunk.destructionCount);
                if (skunk.state.CURRENT_STATE == StateMachine.State.PhaseChange)
                {
                    skunk.isPhaseChanged = true;
                }
            }
            else
                collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal);

            PartDestructionGauge(collision, destructionGauge);
        }
        else if (collision.tag == "DestroyableObject ")
        {
            Destroy(collision.gameObject);
        }
    }

    protected void PartDestructionGauge(Collider2D other, float gauge)
    {
        Skunk boss = other.GetComponent<Skunk>();
        if (boss != null)
        {
            boss.DestructionGauge -= gauge;
        }
    }
}
