using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class absorbObject : MonoBehaviour
{
    [Header("사이즈")]
    [SerializeField]
    private absorb.objectSize size;

    [Header("흡수시간")]
    [SerializeField]
    private float absorbTime;

    [Header("범위 밖 유지시간")]
    public float absorbKeepTime = 1f;

    public bool inAbsorbArea;

    public float speed = 1f;         // 흔들리는 속도
    public float maxSpeed = 10f;     // 최대 속도
    public float acceleration = 1f;  // 가속도

    private Quaternion initialRotation;  // 초기 회전값
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
            currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, maxSpeed);  // 속도 계산
            float yRotation = Mathf.Sin(Time.time * currentSpeed * speed) * currentSpeed;           // 회전값 계산
            transform.rotation = initialRotation * Quaternion.Euler(0, yRotation, 0);              // 회전값 적용

        }
        else
        {
            currentSpeed = 0;
        }
    }


}
