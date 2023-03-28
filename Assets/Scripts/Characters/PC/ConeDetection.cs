using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    [Tooltip("공격력")]
    [SerializeField] private int damage;
    [Tooltip("공격 딜레이")]
    [SerializeField] private float attackDelay = 1f;
    private float lastAttackTime;

    [Space]
    [Tooltip("검출 각도")]
    [SerializeField] private float detectionAngle;
    [Tooltip("검출 거리")]
    [SerializeField] private float detectionDistance;
    [Tooltip("검출 레이어")]
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private List<GameObject> targetList = new List<GameObject>();

    private void Update()
    {
        Vector3 mouseDirection = GetMouseDirection();
        Debug.DrawRay(transform.position, mouseDirection * detectionDistance);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(detectionAngle / 2, Vector3.up) * mouseDirection * detectionDistance);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-detectionAngle / 2, Vector3.up) * mouseDirection * detectionDistance);

        TargetDetection(mouseDirection);
    }

    private Vector3 EulerToVector(float angle)
    {
        angle += transform.eulerAngles.y;
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    private void TargetDetection(Vector3 mouseDirection)
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, detectionDistance, detectionLayer);

        targetList.Clear();

        var radianRange = Mathf.Cos((detectionAngle / 2) * Mathf.Deg2Rad);

        for (int i = 0; i < objs.Length; i++)
        {
            var targetPosition = objs[i].transform.position;
            var targetDirection = (targetPosition - transform.position).normalized;
            var targetRadian = Vector3.Dot(mouseDirection, targetDirection);
            var distance = Vector3.Distance(transform.position, targetPosition);
            AttackEnemy(objs, radianRange, i, targetPosition, targetRadian, distance);
        }
    }

    private void AttackEnemy(Collider[] objs, float radianRange, int Length, Vector3 targetPosition, float targetRadian, float distance)
    {
        if (targetRadian > radianRange)
        {
            var damagePercent = 1f;

            if (distance > detectionDistance * 0.6f)
                damagePercent = 0.6f;

            var damageAmount = damagePercent * damage;

            targetList.Add(objs[Length].gameObject);
            Debug.DrawLine(transform.position, targetPosition, Color.red);

            if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackDelay)
            {
                objs[Length].gameObject.GetComponentInParent<Enemy>().CurrentHp -= Mathf.RoundToInt(damageAmount);
                lastAttackTime = Time.time;
            }
        }
    }

    private Vector3 GetMouseDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var mouseDirection = (hit.point - transform.position).normalized;
            mouseDirection.y = 0; // y값을 0으로 고정
            return mouseDirection;
        }

        return Vector3.zero;
    }
}