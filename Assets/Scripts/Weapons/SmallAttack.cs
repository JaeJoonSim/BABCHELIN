using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Health;

public class SmallAttack : BaseMonoBehaviour
{
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

        if ( AttackTime > PlayerController.TotalStatus.sk1Spd) 
        {
            AttackTime = 0;
            Attack();
        }

    }

    private void Attack()
    {
        //PlayerController.addBullet(-Cost);
        Collider2D[] targetInRange = Physics2D.OverlapCircleAll(transform.position, PlayerController.TotalStatus.sk1Range, 1 << 8);
        for (int i = 0; i < targetInRange.Length; i++)
        {
            Vector2 dirToTarget = (targetInRange[i].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.right, dirToTarget) <= PlayerController.TotalStatus.sk1Angle / 2)
            {
                Skunk boss = targetInRange[i].GetComponent<Skunk>();
                Vector3 collisionPoint = targetInRange[i].ClosestPoint(transform.position);
                collisionPoint += new Vector3(0, 0, -1);
                if (boss != null)
                    targetInRange[i].GetComponent<Health>().Damaged(gameObject, collisionPoint, PlayerController.TotalStatus.sk1Dmg, Health.AttackType.Normal, boss.destructionCount);
                else
                    targetInRange[i].GetComponent<Health>().Damaged(gameObject, collisionPoint, PlayerController.TotalStatus.sk1Dmg, Health.AttackType.Normal);
                PartDestructionGauge(targetInRange[i], PlayerController.TotalStatus.sk1DestroyDmg);
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
