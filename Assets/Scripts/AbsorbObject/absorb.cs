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

    [Header("사이즈별 탄환 충전량")]

    public int addBulletSmall = 20;

    public int addBulletMedium = 50;

    public int addBulletLarge = 100;

    [Header("범위 밖 유지시간")]
    public float absorbKeepTime = 1f;

    [Header("흡수 위치")]
    private Transform player;
    public Transform Player { get { return player; } set { player = value; } }
    [Header("흡수 속도")]
    public float speed;

    private void Start()
    {
        if(Player == null)
        {
            Player = GameObject.FindWithTag("Player").transform;
        }
    }
}
