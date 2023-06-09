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

    [Tooltip("최대 체력")]
    public Stat<int> hpMax;
    [Tooltip("체력 자연치유량")]
    public Stat<int> hpRegen;

    [Space, Tooltip("방어력(%연산)")]
    public Stat<int> def;

    [Space, Tooltip("이동속도")]
    public Stat<float> movSpd;

    [Space, Tooltip("구르기 쿨타임")]
    public Stat<float> dodgeCoolDown;
    [Tooltip("구르기 시간")]
    public Stat<float> dodgeTime;
    [Tooltip("구르기 거리")]
    public Stat<float> dodgeDistance;

    //흡수관련
    [Header("Absorb"), Space]

    public Stat<float> absorbAngle;
    public Stat<float> absorbRange;

    [Tooltip("흡수 충전량(%단위) // 20% 기준 - 충전값(10) + 20%(2))")]
    public Stat<int> absorbRestore;
    [Tooltip("오브젝트 흡수 속도(공통)")]
    public Stat<float> absorbSpd;
    [Tooltip("오브젝트 흡수 속도(소)")]
    public Stat<float> absorbSpdSmall;
    [Tooltip("오브젝트 흡수 속도(중)")]
    public Stat<float> absorbSpdMedium;
    [Tooltip("오브젝트 흡수 속도(대)")]
    public Stat<float> absorbSpdLarge;

    [Tooltip("오브젝트 흡수량(소)")]
    public Stat<float> absorbChargeSmall;
    [Tooltip("오브젝트 흡수량(중)")]
    public Stat<float> absorbChargeMedium;
    [Tooltip("오브젝트 흡수 량(대)")]
    public Stat<float> absorbChargeLarge;

    //공격관련
    [Header("ATK"), Space]

    [Tooltip("기본 공격력")]
    public Stat<float> atk;
    [Tooltip("기본 공격 파괴 데미지")]
    public Stat<float> atkDestroyDmg;
    [Tooltip("공격속도")]
    public Stat<float> atkSpd;
    [Tooltip("기본 공격 발사 갯수")]
    public Stat<int> bulletCount;
    [Tooltip("투사체 속도")]
    public Stat<int> bulletSpd;
    [Tooltip("기본 공격 사정거리")]
    public Stat<int> bulletRange;
    [Tooltip("기본 공격 소모량")]
    public Stat<int> bulletCost;

    [Space, Tooltip("총알 최대치")]
    public Stat<int> bulletMax;
    [Tooltip("총알 최소치")]
    public Stat<int> bulletMin;
    [Tooltip("탄환 자연회복량(최소치까지)")]
    public Stat<int> bulletRegen;
    [Tooltip("탄환 자연시간(초)(최소치까지)")]
    public Stat<int> bulletRegenTime;
    [Tooltip("가장 가까운 적에게 유도탄")]
    public Stat<bool> bulletAuto;
    [Tooltip("유도 범위")]
    public Stat<float> bulletAutoRange;

    //소형탄관련
    [Header("Skill_1(소형탄)"), Space]

    [Tooltip("소형탄 데미지")]
    public Stat<float> sk1Dmg;
    [Tooltip("기본 공격 소모량")]
    public Stat<int> sk1Cost;
    [Tooltip("소형탄 쿨타임")]
    public Stat<float> sk1CoolDown;
    [Tooltip("소형탄 지속시간")]
    public Stat<float> sk1ResistTime;
    [Tooltip("소형탄 공격속도")]
    public Stat<float> sk1Spd;
    [Tooltip("소형탄 파괴 데미지")]
    public Stat<float> sk1DestroyDmg;
    [Tooltip("소형탄 사거리")]
    public Stat<float> sk1Range;
    [Tooltip("소형탄 범위(각도)")]
    public Stat<float> sk1Angle;


    //대형탄관련
    [Header("Skill_2(대형탄)"), Space]

    [Tooltip("대형탄 데미지")]
    public Stat<float> sk2Dmg;
    [Tooltip("기본 공격 소모량")]
    public Stat<int> sk2Cost;
    [Tooltip("대형탄 쿨타임")]
    public Stat<float> sk2CoolDown;
    [Tooltip("대형탄 사거리")]
    public Stat<float> sk2Range;
    [Tooltip("대형탄 파괴 데미지")]
    public Stat<float> sk2DestroyDmg;
    [Tooltip("대형탄 스플래쉬 범위")]
    public Stat<float> sk2SplashRange;
    [Tooltip("대형탄 스플래쉬 데미지")]
    public Stat<float> sk2SplashDmg;
    [Tooltip("대형탄 충전횟수")]
    public Stat<float> sk2Count;
    [Tooltip("대형탄 가장 가까운 적에게 유도탄")]
    public Stat<bool> sk2Auto;
    [Tooltip("대형탄 유도 범위")]
    public Stat<float> sk2AutoRange;


    //궁극기관련
    [Header("ultimate"), Space]

    [Tooltip("궁극기 충전량(%단위) // 20% 기준 - 충전값(10) + 20%(2))")]
    public Stat<int> ultRestore;
    [Tooltip("n% 확률로 궁극기 사용 후 게이지 100% 회복")]
    public Stat<float> ultTwice;
    [Tooltip("궁극기 최대치")]
    public Stat<int> UltMax;

    [Header("ohter"), Space]
    [Tooltip("부활 횟수")]
    public Stat<int> Revive;
    [Tooltip("부활시 체력")]
    public Stat<int> ReviveHp;
    [Tooltip("피흡 량")]
    public Stat<float> hpAbsorb;


    public Dictionary<string, dynamic> variables;

    public void SaveFieldsToVariables()
    {
        variables = new Dictionary<string, dynamic>();
        variables.Clear();
        // 현재 구조체 타입
        Type structType = typeof(status);

        // 모든 필드 가져오기
        FieldInfo[] fields = structType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // 필드 이름 가져오기
            string fieldName = field.Name;

            if (!variables.ContainsKey(fieldName))
                variables.Add(fieldName, field.GetValue(this));
        }
    }

    public void ReSaveFieldsToVariables()
    {
        // 현재 구조체 타입
        Type structType = typeof(status);

        // 모든 필드 가져오기
        FieldInfo[] fields = structType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // 필드 이름 가져오기
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
