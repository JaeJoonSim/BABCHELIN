using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : BaseMonoBehaviour
{
    public float Damage;
    public float speed;
    public float range;
    public int Cost;
    public bool bulletAuto;
    public float bulletAutoRange;
    public float destructionGauge;
    Vector2 spownPos;
    public ParticleSystem HitEffet;
    public ParticleSystem endEffet;
    private DamageTextControler DmgTextController;

    private Transform target;

    private void Start()
    {
        target = null;
        spownPos = transform.position;
    }
    void Update()
    {
        if (bulletAuto)
        {
            if (target == null)
                target = SearchTarget();
            else
            {
                float auto = transform.eulerAngles.z;

                auto = Mathf.Lerp(auto, Utils.GetAngle(base.transform.position, target.transform.position), speed / 3 * Time.deltaTime);

                transform.rotation = Quaternion.Euler(0, 0, auto);

            }
        }

        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, spownPos) > range)
        {
            Instantiate(endEffet, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    public virtual void getStaus(status val)
    {
        Damage = val.atk.value;
        speed = val.bulletSpd.value;
        range = val.bulletRange.value;
        Cost = val.bulletCost.value;
        destructionGauge = 0;
        bulletAuto = val.bulletAuto.value;    
        bulletAutoRange = val.bulletAutoRange.value;
    }

    private Transform SearchTarget()
    {
        Collider2D targetInRange = Physics2D.OverlapCircle(transform.position, bulletAutoRange, 1 << 8);
        if (targetInRange != null)
            return targetInRange.transform;

        return null;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Debug.Log("Hit Test");
            Skunk skunk = collision.GetComponent<Skunk>();
            Vector3 collisionPoint = collision.ClosestPoint(transform.position);
            if (skunk != null)
                collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal, skunk.destructionCount);
            else
                collision.GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal);

            PartDestructionGauge(collision, destructionGauge);
            DmgTextController = collision.gameObject.GetComponent<DamageTextControler>();
            if(DmgTextController != null)
            {
                DmgTextController.ShowDamageText(Damage);
            }
            else
            {

            }
            Instantiate(HitEffet, collisionPoint, Quaternion.identity);
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

    protected virtual void PartDestructionGauge(Collider2D other, float gauge)
    {
        Skunk boss = other.GetComponent<Skunk>();
        if (boss != null)
        {
            boss.DestructionGauge -= gauge;
        }
    }
}
