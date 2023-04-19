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

    [Header("����� źȯ ������")]

    public int addBulletSmall = 20;

    public int addBulletMedium = 50;

    public int addBulletLarge = 100;

    [Header("���� �� �����ð�")]
    public float absorbKeepTime = 1f;

    [Header("��� ��ġ")]
    private Transform player;
    public Transform Player { get { return player; } set { player = value; } }
    [Header("��� �ӵ�")]
    public float speed;

    private void Start()
    {
        if(Player == null)
        {
            Player = GameObject.FindWithTag("Player").transform;
        }
    }
}
