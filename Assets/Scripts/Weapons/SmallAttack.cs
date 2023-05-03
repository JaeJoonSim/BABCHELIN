using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SmallAttack : BaseMonoBehaviour
{
    public float Angle = 30f;
    public float Range = 10f;

    void Start()
    {
        Collider2D[] targetInRange = Physics2D.OverlapCircleAll(transform.position, Range, 1 << 8);
        Debug.Log(targetInRange.Length);
        for (int i = 0; i < targetInRange.Length; i++)
        {
            Vector2 dirToTarget = (targetInRange[i].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.right, dirToTarget) <= Angle / 2)
            {
                targetInRange[i].GetComponent<Health>().Damaged(gameObject, transform.position, 2f, Health.AttackType.Normal);
                Debug.Log(targetInRange[i].name);
                Debug.DrawLine(transform.position, targetInRange[i].transform.position, Color.green);
            }
        }

        Destroy(gameObject);
    }

   


}
