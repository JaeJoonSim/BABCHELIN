using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : BaseMonoBehaviour
{
    public float Damage;
    public float speed;
    public float range;
    public int Cost;
    public float destructionGauge;
    Vector2 spownPos;
    public ParticleSystem HitEffet;
    private DamageTextControler DmgTextController;

    private void Start()
    {
        spownPos = transform.position;
    }
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, spownPos) > range)
        {
            Destroy(gameObject);
        }
    }

    public virtual void getStaus(status val)
    {
        Damage = val.atk;
        speed = val.atkSpd / 10;
        range = val.bulletRange;
        Cost = val.bulletCost;
        destructionGauge = 0; ;
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
