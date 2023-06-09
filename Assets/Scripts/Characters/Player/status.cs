using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Stat<T>
{
    public T value;

    public Stat(T value)
    {
        this.value = value;
    }
}


[System.Serializable]
public struct status
{
    //PC Base
    [Header("PC Base"), Space]

    [Tooltip("�ִ� ü��")]
    public Stat<int> hpMax;
    [Tooltip("ü�� �ڿ�ġ����")]
    public Stat<int> hpRegen;

    [Space, Tooltip("����(%����)")]
    public Stat<int> def;

    [Space, Tooltip("�̵��ӵ�")]
    public Stat<float> movSpd;

    [Space, Tooltip("������ ��Ÿ��")]
    public Stat<float> dodgeCoolDown;
    [Tooltip("������ �ð�")]
    public Stat<float> dodgeTime;
    [Tooltip("������ �Ÿ�")]
    public Stat<float> dodgeDistance;

    //�������
    [Header("Absorb"), Space]

    public Stat<float> absorbAngle;
    public Stat<float> absorbRange;

    [Tooltip("��� ������(%����) // 20% ���� - ������(10) + 20%(2))")]
    public Stat<int> absorbRestore;
    [Tooltip("������Ʈ ��� �ӵ�(����)")]
    public Stat<float> absorbSpd;
    [Tooltip("������Ʈ ��� �ӵ�(��)")]
    public Stat<float> absorbSpdSmall;
    [Tooltip("������Ʈ ��� �ӵ�(��)")]
    public Stat<float> absorbSpdMedium;
    [Tooltip("������Ʈ ��� �ӵ�(��)")]
    public Stat<float> absorbSpdLarge;

    [Tooltip("������Ʈ �����(��)")]
    public Stat<float> absorbChargeSmall;
    [Tooltip("������Ʈ �����(��)")]
    public Stat<float> absorbChargeMedium;
    [Tooltip("������Ʈ ��� ��(��)")]
    public Stat<float> absorbChargeLarge;

    //���ݰ���
    [Header("ATK"), Space]

    [Tooltip("�⺻ ���ݷ�")]
    public Stat<float> atk;
    [Tooltip("�⺻ ���� �ı� ������")]
    public Stat<float> atkDestroyDmg;
    [Tooltip("���ݼӵ�")]
    public Stat<float> atkSpd;
    [Tooltip("�⺻ ���� �߻� ����")]
    public Stat<int> bulletCount;
    [Tooltip("����ü �ӵ�")]
    public Stat<int> bulletSpd;
    [Tooltip("�⺻ ���� �����Ÿ�")]
    public Stat<int> bulletRange;
    [Tooltip("�⺻ ���� �Ҹ�")]
    public Stat<int> bulletCost;

    [Space, Tooltip("�Ѿ� �ִ�ġ")]
    public Stat<int> bulletMax;
    [Tooltip("�Ѿ� �ּ�ġ")]
    public Stat<int> bulletMin;
    [Tooltip("źȯ �ڿ�ȸ����(�ּ�ġ����)")]
    public Stat<int> bulletRegen;
    [Tooltip("źȯ �ڿ��ð�(��)(�ּ�ġ����)")]
    public Stat<int> bulletRegenTime;
    [Tooltip("���� ����� ������ ����ź")]
    public Stat<bool> bulletAuto;
    [Tooltip("���� ����")]
    public Stat<float> bulletAutoRange;

    //����ź����
    [Header("Skill_1(����ź)"), Space]

    [Tooltip("����ź ������")]
    public Stat<float> sk1Dmg;
    [Tooltip("�⺻ ���� �Ҹ�")]
    public Stat<int> sk1Cost;
    [Tooltip("����ź ��Ÿ��")]
    public Stat<float> sk1CoolDown;
    [Tooltip("����ź ���ӽð�")]
    public Stat<float> sk1ResistTime;
    [Tooltip("����ź ���ݼӵ�")]
    public Stat<float> sk1Spd;
    [Tooltip("����ź �ı� ������")]
    public Stat<float> sk1DestroyDmg;
    [Tooltip("����ź ��Ÿ�")]
    public Stat<float> sk1Range;
    [Tooltip("����ź ����(����)")]
    public Stat<float> sk1Angle;


    //����ź����
    [Header("Skill_2(����ź)"), Space]

    [Tooltip("����ź ������")]
    public Stat<float> sk2Dmg;
    [Tooltip("�⺻ ���� �Ҹ�")]
    public Stat<int> sk2Cost;
    [Tooltip("����ź ��Ÿ��")]
    public Stat<float> sk2CoolDown;
    [Tooltip("����ź ��Ÿ�")]
    public Stat<float> sk2Range;
    [Tooltip("����ź �ı� ������")]
    public Stat<float> sk2DestroyDmg;
    [Tooltip("����ź ���÷��� ����")]
    public Stat<float> sk2SplashRange;
    [Tooltip("����ź ���÷��� ������")]
    public Stat<float> sk2SplashDmg;
    [Tooltip("����ź ����Ƚ��")]
    public Stat<float> sk2Count;
    [Tooltip("����ź ���� ����� ������ ����ź")]
    public Stat<bool> sk2Auto;
    [Tooltip("����ź ���� ����")]
    public Stat<float> sk2AutoRange;


    //�ñر����
    [Header("ultimate"), Space]

    [Tooltip("�ñر� ������(%����) // 20% ���� - ������(10) + 20%(2))")]
    public Stat<int> ultRestore;
    [Tooltip("n% Ȯ���� �ñر� ��� �� ������ 100% ȸ��")]
    public Stat<float> ultTwice;
    [Tooltip("�ñر� �ִ�ġ")]
    public Stat<int> UltMax;

    [Header("ohter"), Space]
    [Tooltip("��Ȱ Ƚ��")]
    public Stat<int> Revive;
    [Tooltip("��Ȱ�� ü��")]
    public Stat<int> ReviveHp;
    [Tooltip("���� ��")]
    public Stat<float> hpAbsorb;


    public Dictionary<string, dynamic> variables;

    public void SaveFieldsToVariables()
    {
        variables = new Dictionary<string, dynamic>();
        variables.Clear();
        // ���� ����ü Ÿ��
        Type structType = typeof(status);

        // ��� �ʵ� ��������
        FieldInfo[] fields = structType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // �ʵ� �̸� ��������
            string fieldName = field.Name;

            if (!variables.ContainsKey(fieldName))
                variables.Add(fieldName, field.GetValue(this));
        }
    }

    public void ReSaveFieldsToVariables()
    {
        // ���� ����ü Ÿ��
        Type structType = typeof(status);

        // ��� �ʵ� ��������
        FieldInfo[] fields = structType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // �ʵ� �̸� ��������
            Type fieldType = field.FieldType;
            
            if(fieldType == typeof(Stat<int>))
            {
                field.SetValue(this, new Stat<int>(0));
            }
            else if (fieldType == typeof(Stat<float>))
            {
                field.SetValue(this, new Stat<float>(0));
            }
            else if (fieldType == typeof(Stat<bool>))
            {
                field.SetValue(this, new Stat<bool>(false));
            }
        }
    }

}
