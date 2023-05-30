using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct status
{
    //PC Base
    [Header("PC Base"), Space]

    [Tooltip("�ִ� ü��")]
    public int hpMax;
    [Tooltip("ü�� �ڿ�ġ����")]
    public int hpRegen;

    [Space, Tooltip("����(%����)")]
    public int def;

    [Space,Tooltip("�̵��ӵ�")]
    public float movSpd;

    [Space, Tooltip("������ ��Ÿ��")]
    public float dodgeCoolDown;
    [Tooltip("������ �ӵ�")]
    public float dodgeSpeed;

    //�������
    [Header("Absorb"), Space]

    public float absorbAngle;
    public float absorbRange;

    [Tooltip("��� ������(%����) // 20% ���� - ������(10) + 20%(2))")]
    public int absorbRestore;
    [Tooltip("������Ʈ ��� �ӵ�(��)")]
    public float absorbSpdSmall;
    [Tooltip("������Ʈ ��� �ӵ�(��)")]
    public float absorbSpdMedium;
    [Tooltip("������Ʈ ��� �ӵ�(��)")]
    public float absorbSpdLarge;

    [Tooltip("������Ʈ �����(��)")]
    public float absorbChargeSmall;
    [Tooltip("������Ʈ �����(��)")]
    public float absorbChargeMedium;
    [Tooltip("������Ʈ ��� �ӵ�(��)")]
    public float absorbChargeLarge;

    //���ݰ���
    [Header("ATK"), Space]

    [Tooltip("�⺻ ���ݷ�")]
    public float atk;
    [Tooltip("���ݼӵ�")]
    public float atkSpd;
    [Tooltip("�⺻ ���� �߻� ����")]
    public int bulletCount;
    [Tooltip("�⺻ ���� �����Ÿ�")]
    public int bulletRange;
    [Tooltip("�⺻ ���� �Ҹ�")]
    public int bulletCost;

    [Space, Tooltip("�Ѿ� �ִ�ġ")]
    public int bulletMax;
    [Tooltip("�Ѿ� �ּ�ġ")]
    public int bulletMin;
    [Tooltip("źȯ �ڿ�ȸ����(�ּ�ġ����)")]
    public int bulletRegen;
    [Tooltip("źȯ �ڿ��ð�(��)(�ּ�ġ����)")]
    public int bulletRegenTime;

    //����ź����
    [Header("Skill_1(����ź)"), Space]

    [Tooltip("����ź ������")]
    public float sk1Dmg;
    [Tooltip("�⺻ ���� �Ҹ�")]
    public int sk1Cost;
    [Tooltip("����ź ��Ÿ��")]
    public float sk1CoolDown;
    [Tooltip("����ź ���ӽð�")]
    public float sk1ResistTime;
    [Tooltip("����ź ���ݼӵ�")]
    public float sk1Spd;
    [Tooltip("����ź �ı� ������")]
    public float sk1DestroyDmg;
    [Tooltip("����ź ��Ÿ�")]
    public float sk1Range;
    [Tooltip("����ź ����(����)")]
    public float sk1Angle;


    //����ź����
    [Header("Skill_1(����ź)"), Space]

    [Tooltip("����ź ������")]
    public float sk2Dmg;
    [Tooltip("�⺻ ���� �Ҹ�")]
    public int sk2Cost;
    [Tooltip("����ź ��Ÿ��")]
    public float sk2CoolDown;
    [Tooltip("����ź ��Ÿ�")]
    public float sk2Range;
    [Tooltip("����ź �ı� ������")]
    public float sk2DestroyDmg;
    [Tooltip("����ź ���÷��� ����")]
    public float sk2SplashRange;
    [Tooltip("����ź ���÷��� ������")]
    public float sk2SplashDmg;
    [Tooltip("����ź ����Ƚ��")]
    public float sk2Count;
    

    //�ñر����
    [Header("ultimate"), Space]

    [Tooltip("�ñر� ������(%����) // 20% ���� - ������(10) + 20%(2))")]
    public int ultRestore;
    [Tooltip("�ñر� �ִ�ġ")]
    public int UltMax;

}
