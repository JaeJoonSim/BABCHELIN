using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class absorbObject : MonoBehaviour
{
    [Header("������")]
    [SerializeField]
    private absorb.objectSize size;

    [Header("����ð�")]
    [SerializeField]
    private float absorbTime;

    [Header("���� �� �����ð�")]
    public float absorbKeepTime = 1f;

    public bool inAbsorbArea;

    public float speed = 1f;         // ��鸮�� �ӵ�
    public float maxSpeed = 10f;     // �ִ� �ӵ�
    public float acceleration = 1f;  // ���ӵ�

    private Quaternion initialRotation;  // �ʱ� ȸ����
    private float currentSpeed;

    void Start()
    {
        switch (size)
        {
            case absorb.objectSize.small:
                absorbTime = absorb.Instance.absorbTimeSmall;
                break;
            case absorb.objectSize.medium:
                absorbTime = absorb.Instance.absorbTimeMedium;
                break;
            case absorb.objectSize.large:
                absorbTime = absorb.Instance.absorbTimeLarge;
                break;
            default:
                break;
        }

        absorbKeepTime = absorb.Instance.absorbKeepTime;

        inAbsorbArea = false;

        initialRotation = transform.rotation;


    }

    private void Update()
    {
        if (inAbsorbArea)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, maxSpeed);  // �ӵ� ���
            float yRotation = Mathf.Sin(Time.time * currentSpeed * speed) * currentSpeed;           // ȸ���� ���
            transform.rotation = initialRotation * Quaternion.Euler(0, yRotation, 0);              // ȸ���� ����

        }
        else
        {
            currentSpeed = 0;
        }
    }


}
