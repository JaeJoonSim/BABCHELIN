using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeAttack : PlayerAttack
{
    public float splashRange;
    public float splashDmg;
    public override void getStaus(status val)
    {
        Damage = val.sk2Dmg.value;
        speed = val.bulletSpd.value;
        range = val.sk2Range.value;
        Cost = val.sk2Cost.value;
        destructionGauge = val.sk2DestroyDmg.value;
        splashRange = val.sk2SplashRange.value;
        splashDmg = val.sk2SplashDmg.value;
        bulletAuto = val.bulletAuto.value;
        bulletAutoRange = val.bulletAutoRange.value;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Vector3 collisionPoint = collision.ClosestPoint(transform.position);
            collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal);
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
