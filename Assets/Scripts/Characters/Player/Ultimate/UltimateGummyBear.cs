using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateGummyBear : MonoBehaviour
{
    public float Damage;
    public float destructionGauge;
    public float size;

    public ParticleSystem endEffet;
    public SkeletonAnimation Spine;
    private void Start()
    {
        Invoke("Destroyobj", 6f);
        Spine.AnimationState.Event += OnSpineEvent;
    }

    public void getStatus(UltimateStatus val)
    {
        Damage = val.Damage;
        destructionGauge = val.destructionGauge;
        transform.localScale = Vector3.one * val.size;
        size = val.size;
    }

    private void Destroyobj()
    {
        Instantiate(endEffet, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        if (collision.CompareTag("EnemyAttack"))
            Destroy(collision.gameObject);
        else if (collision.CompareTag("DestroyableObject "))
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<HealthPlayer>().untouchable = true;
            Debug.Log("안전합니다.");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<HealthPlayer>().untouchable = false;
            Debug.Log("위험합니다.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("EnemyAttack"))
            Destroy(collision.gameObject);
        else if (collision.gameObject.CompareTag("DestroyableObject "))
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if(e.Data.Name == "bump")
        {
            GetComponent<CircleCollider2D>().enabled = true;

            Collider2D[] targetInRange = Physics2D.OverlapCircleAll(transform.position, size, 1 << 8);
            for (int i = 0; i < targetInRange.Length; i++)
            {
                    Skunk skunk = targetInRange[i].GetComponent<Skunk>();
                    Vector3 collisionPoint = targetInRange[i].ClosestPoint(transform.position);
                    if (skunk != null)
                    {
                    targetInRange[i].GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal, skunk.destructionCount);
                        if (skunk.state.CURRENT_STATE == StateMachine.State.PhaseChange)
                        {
                            skunk.isPhaseChanged = true;
                        }
                    }
                    else
                    targetInRange[i].GetComponent<Health>().Damaged(gameObject, collisionPoint, Damage, Health.AttackType.Normal);

                    PartDestructionGauge(targetInRange[i], destructionGauge);

            }

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
