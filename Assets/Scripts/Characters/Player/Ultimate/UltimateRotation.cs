using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateRotation : MonoBehaviour
{
    Quaternion startRotation;
    Quaternion endRotation;
    float timer;
    bool start;
    float duration = 0.5f; // 전체 시간

    void Start()
    {
        startRotation = transform.localRotation;
        endRotation = Quaternion.Euler(0f, -90f, -90f);
        timer = 0f;
        start = false;

    }

    // Update is called once per frame
    public bool RotateObj()
    {
        timer += Time.deltaTime;
        if (!start)
        {
            if (timer > 2f)
            {
                start = true;
                timer = 0f;
            }
        }
        else
        {
            if (timer <= duration)
            {
                float t = Mathf.SmoothStep(0f, 1f, timer / duration);
                transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            }
            else
                return true;
        }

        return false;
    }
}
