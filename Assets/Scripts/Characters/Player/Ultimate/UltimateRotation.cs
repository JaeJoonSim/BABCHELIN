using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UltimateRotation : MonoBehaviour
{
    public float duration = 3f;
    public AnimationCurve accelerationCurve;

    private float currentTime = 0f;

    [HideInInspector]
    public bool start;
    void Start()
    {
        start = false;
        currentTime = 0f;
    }


    private void Update()
    {
        currentTime += Time.deltaTime;

        if (1.5f < currentTime && currentTime < duration + 1.5f)
        {   
            float t = (currentTime - 1.5f) / duration;
            float acceleration = accelerationCurve.Evaluate(t);
            float currentAngle = Mathf.Lerp(0, -90, t) * acceleration;
            transform.localRotation = Quaternion.Euler(0f, currentAngle, -90f);
        }
        else if (currentTime > duration + 1.5f)
        {
            start = true;
        }

    }
}