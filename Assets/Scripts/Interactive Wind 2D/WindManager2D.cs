using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager2D : MonoBehaviour
{
    [Header("Settings:")]

    [Tooltip("값이 작을수록 바람의 변화가 더 자주 발생")]
    public float windNoiseScale = 0.1f;
    private float lastWindNoiseScale;

    [Tooltip("바람 패턴이 수평으로 이동하는 속도입니다.")]
    public float windNoiseSpeed = 0.2f;
    private float lastWindNoiseSpeed;

    [Tooltip("바람의 세기는 이 값과 'windIntensityTo' 사이가 됩니다.")]
    public float windIntensityFrom = -0.5f;
    private float lastWindIntensityFrom;

    [Tooltip("바람의 세기는 이 값과 'windIntensityFrom' 사이가 됩니다.")]
    public float windIntensityTo = 0.5f;
    private float lastWindIntensityTo;

    float currentTime;

    void Start()
    {
        currentTime = 0;
    }

    void FixedUpdate()
    {
        ModifyIfChanged(ref windNoiseScale, ref lastWindNoiseScale, "WindNoiseScale");
        ModifyIfChanged(ref windIntensityFrom, ref lastWindIntensityFrom, "WindMinIntensity");
        ModifyIfChanged(ref windIntensityTo, ref lastWindIntensityTo, "WindMaxIntensity");

        currentTime += Time.fixedDeltaTime * windNoiseSpeed;
        Shader.SetGlobalFloat("WindTime", currentTime);
    }

    public void ModifyIfChanged(ref float currentValue, ref float oldValue, string globalShaderName)
    {
        if (oldValue != currentValue)
        {
            oldValue = currentValue;
            Shader.SetGlobalFloat(globalShaderName, currentValue);
        }
    }
}