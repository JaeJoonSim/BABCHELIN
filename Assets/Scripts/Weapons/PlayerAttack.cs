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

    [SerializeField]
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
                Vector3 targetdir = (target.position - transform.position).normalized;
                // 내적(dot)을 통해 각도를 구함. (Acos로 나온 각도는 방향을 알 수가 없음)
                float dot = Vector3.Dot(transform.right, targetdir);

                if (dot < 1.0f)
                {
                    float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                    //외적을 통해 각도의 방향을 판별.
                   Vector3 cross = Vector3.Cross(transform.right, targetdir);
                    //외적 결과 값에 따라 각도 반영
                    if (cross.z < 0)
                    {
                        angle = transform.rotation.eulerAngles.z - Mathf.Min(10, angle);
                    }
                    else
                    {
                        angle = transform.rotation.eulerAngles.z + Mathf.Min(10, angle);
                    }
                    angle = Mathf.Lerp(transform.eulerAngles.z, angle, speed * 2  * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                    //angle이 윗 방향과 target의 각도.
                      // do someting.
                    //Debug.Log(angle);
                }

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
        destructionGauge = val.atkDestroyDmg.value;
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
            Debug.Log("123");
        }
        
    }
}
