using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class absorb : Singleton<absorb>
{
    public enum objectSize { small, medium, large };

    [Header("사이즈별 흡수시간")]

    public float absorbTimeSmall = 0.1f;

    public float absorbTimeMedium = 0.5f;

    public float absorbTimeLarge = 1f;

    [Header("범위 밖 유지시간")]
    public float absorbKeepTime = 1f;



}
