using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Health;

public class SmallAttack : BaseMonoBehaviour
{
    public float Angle = 30f;
    public float Range = 10f;
    public float Damage;
    public float AttackSpeed;
    public int Cost;
    public float destructionGauge;

    public ParticleSystem HitEffet;

    private float AttackTime;
    private PlayerController playerController;
    public PlayerController PlayerController
    {
        get
        {
            if (playerController == null)
            {
                playerController = transform.root.gameObject.GetComponent<PlayerController>();
            }
            return playerController;
        }
    }

    private void OnEnable()
    {
        AttackTime = 0;
        Attack();
    }

    private void Update()
    {
        AttackTime += Time.deltaTime;

        if ( AttackTime > AttackSpeed ) 
        {
            AttackTime = 0;
            Attack();
        }

    }

    private void Attack()
    {
        PlayerController.addBullet(-Cost);
        Collider2D[] targetInRange = Physics2D.OverlapCircleAll(transform.position, Range, 1 << 8);
        for (int i = 0; i < targetInRange.Length; i++)
        {
            Vector2 dirToTarget = (targetInRange[i].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.right, dirToTarget) <= Angle / 2)
            {
                Skunk boss = targetInRange[i].GetComponent<Skunk>();
                Vector3 collisionPoint = targetInRange[i].ClosestPoint(transform.position);
                collisionPoint += new Vector3(0, 0, -1);
                if (boss != null)
                    targetInRange[i].GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal, boss.destructionCount);
                else
                    targetInRange[i].GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal);
                PartDestructionGauge(targetInRange[i], destructionGauge);
                Instantiate(HitEffet, collisionPoint, Quaternion.identity);               
            }
        }
    }

    private void PartDestructionGauge(Collider2D other, float gauge)
    {
        Skunk boss = other.GetComponent<Skunk>();
        if (boss != null)
        {
            boss.DestructionGauge -= gauge;
        }
    }
}
