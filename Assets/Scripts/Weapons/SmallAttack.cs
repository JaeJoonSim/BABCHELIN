using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SmallAttack : BaseMonoBehaviour
{
    public float SuctionAngle = 30f;
    public float SuctionRange = 10f;

    void Start()
    {
        Collider2D[] targetInRange = Physics2D.OverlapCircleAll(transform.position, SuctionRange, 1 << 8);
        Debug.Log(targetInRange.Length);
        for (int i = 0; i < targetInRange.Length; i++)
        {
            Vector2 dirToTarget = (targetInRange[i].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.right, dirToTarget) <= SuctionAngle / 2)
            {
                targetInRange[i].GetComponent<Health>().Damaged(gameObject, transform.position, 2f);
                Debug.Log(targetInRange[i].name);
                Debug.DrawLine(transform.position, targetInRange[i].transform.position, Color.green);
            }
        }

        Destroy(gameObject);
    }

   


}
