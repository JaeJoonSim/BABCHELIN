using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeAttack : PlayerAttack
{
    public float splashRange;
    public float splashDmg;
    public override void getStaus(status val)
    {
        Damage = val.sk2Dmg;
        speed = val.atkSpd / 10;
        range = val.sk2Range;
        Cost = val.sk2Cost;
        destructionGauge = val.sk2DestroyDmg;
        splashRange = val.sk2SplashRange;
        splashDmg = val.sk2SplashDmg;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Vector3 collisionPoint = collision.ClosestPoint(transform.position);
            collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, 10f, Health.AttackType.Normal);
            Instantiate(HitEffet, collisionPoint, Quaternion.identity);

            Collider2D[] targetInRange = Physics2D.OverlapCircleAll(transform.position, splashRange, 1 << 8);
            for (int i = 0; i < targetInRange.Length; i++)
            {
                targetInRange[i].GetComponent<Health>().Damaged(gameObject, collisionPoint, splashDmg, Health.AttackType.Normal);
                PartDestructionGauge(targetInRange[i], destructionGauge);
            }
            Destroy(gameObject);
        }
        else if (collision.tag == "DestroyableObject ")
        {
            Vector3 collisionPoint = collision.ClosestPoint(transform.position);
            Instantiate(HitEffet, collisionPoint, Quaternion.identity);
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
