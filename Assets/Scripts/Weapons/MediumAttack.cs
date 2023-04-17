using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumAttack : MonoBehaviour
{
    public float Range = 0;
    public float MaxRange = 10f;
    public float Angle = 30f;
    public float Speed = 5f;

    void Update()
    {
        Range += Speed * Time.deltaTime;
        Collider2D[] targetInRange = Physics2D.OverlapCircleAll(transform.position, Range, 1 << 8);
        Debug.Log(targetInRange.Length);
        for (int i = 0; i < targetInRange.Length; i++)
        {
            Vector2 dirToTarget = (targetInRange[i].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.right, dirToTarget) <= Angle / 2)
            {
                if (Vector3.Distance(targetInRange[i].transform.position, transform.position) >= Range-1)
                {
                    targetInRange[i].GetComponent<Health>().Damaged(gameObject, transform.position, 2f);
                    Debug.Log(targetInRange[i].name);
                    Debug.DrawLine(transform.position, targetInRange[i].transform.position, Color.green);
                }
               
            }
        }

        if (Range >= MaxRange)
        {
            Destroy(gameObject);
        }
      
    }
}
