using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class absorb : Singleton<absorb>
{
    public enum objectSize { small, medium, large };

    [Header("����� ����ð�")]

    public float absorbTimeSmall = 0.1f;

    public float absorbTimeMedium = 0.5f;

    public float absorbTimeLarge = 1f;

    [Header("���� �� �����ð�")]
    public float absorbKeepTime = 1f;



}
