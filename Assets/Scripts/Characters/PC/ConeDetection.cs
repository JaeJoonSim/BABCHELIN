using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    [Tooltip("공격력")]
    [SerializeField] private int damage;
    public int Damage
    {
        get { return damage; }
    }
    [Tooltip("공격 딜레이")]
    [SerializeField] private float attackDelay = 1f;
    private float lastAttackTime;
    [Tooltip("공격 이펙트")]
    [SerializeField] private ParticleSystem attackEffect;

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
        PlayEffect(mouseDirection);
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

            if (targetRadian > radianRange)
            {
                var damagePercent = 1f;

                if (distance > detectionDistance * 0.6f)
                    damagePercent = 0.6f;

                var damageAmount = damagePercent * damage;

                targetList.Add(objs[i].gameObject);
                Debug.DrawLine(transform.position, targetPosition, Color.red);

                if (Input.GetMouseButton(0) && Time.time - lastAttackTime >= attackDelay)
                {
                    objs[i].gameObject.GetComponentInParent<Enemy>().CurrentHp -= Mathf.RoundToInt(damageAmount);
                    objs[i].gameObject.GetComponentInParent<Enemy>().hitDamage = Mathf.RoundToInt(damageAmount);
                }
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

    private void PlayEffect(Vector3 mouseDirection)
    {
        if (Input.GetMouseButton(0) && Time.time - lastAttackTime >= attackDelay)
        {
            Quaternion newRotation = Quaternion.LookRotation(mouseDirection);
            attackEffect.transform.rotation = newRotation;

            Vector3 effectPosition = transform.position + mouseDirection;
            attackEffect.transform.position = new Vector3(effectPosition.x, attackEffect.transform.position.y, effectPosition.z);
            attackEffect.Play();
            lastAttackTime = Time.time;
        }
    }
}