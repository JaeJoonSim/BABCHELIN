using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct status
{
    //PC Base
    [Header("PC Base"), Space]

    [Tooltip("최대 체력")]
    public int hpMax;
    [Tooltip("체력 자연치유량")]
    public int hpRegen;

    [Space, Tooltip("방어력(%연산)")]
    public int def;

    [Space,Tooltip("이동속도")]
    public float movSpd;

    [Space, Tooltip("구르기 쿨타임")]
    public float dodgeCoolDown;
    [Tooltip("구르기 속도")]
    public float dodgeSpeed;

    //흡수관련
    [Header("Absorb"), Space]

    public float absorbAngle;
    public float absorbRange;

    [Tooltip("흡수 충전량(%단위) // 20% 기준 - 충전값(10) + 20%(2))")]
    public int absorbRestore;
    [Tooltip("오브젝트 흡수 속도(소)")]
    public float absorbSpdSmall;
    [Tooltip("오브젝트 흡수 속도(중)")]
    public float absorbSpdMedium;
    [Tooltip("오브젝트 흡수 속도(대)")]
    public float absorbSpdLarge;

    [Tooltip("오브젝트 흡수량(소)")]
    public float absorbChargeSmall;
    [Tooltip("오브젝트 흡수량(중)")]
    public float absorbChargeMedium;
    [Tooltip("오브젝트 흡수 속도(대)")]
    public float absorbChargeLarge;

    //공격관련
    [Header("ATK"), Space]

    [Tooltip("기본 공격력")]
    public float atk;
    [Tooltip("공격속도")]
    public float atkSpd;
    [Tooltip("기본 공격 발사 갯수")]
    public int bulletCount;
    [Tooltip("기본 공격 사정거리")]
    public int bulletRange;
    [Tooltip("기본 공격 소모량")]
    public int bulletCost;

    [Space, Tooltip("총알 최대치")]
    public int bulletMax;
    [Tooltip("총알 최소치")]
    public int bulletMin;
    [Tooltip("탄환 자연회복량(최소치까지)")]
    public int bulletRegen;
    [Tooltip("탄환 자연시간(초)(최소치까지)")]
    public int bulletRegenTime;

    //소형탄관련
    [Header("Skill_1(소형탄)"), Space]

    [Tooltip("소형탄 데미지")]
    public float sk1Dmg;
    [Tooltip("기본 공격 소모량")]
    public int sk1Cost;
    [Tooltip("소형탄 쿨타임")]
    public float sk1CoolDown;
    [Tooltip("소형탄 지속시간")]
    public float sk1ResistTime;
    [Tooltip("소형탄 공격속도")]
    public float sk1Spd;
    [Tooltip("소형탄 파괴 데미지")]
    public float sk1DestroyDmg;
    [Tooltip("소형탄 사거리")]
    public float sk1Range;
    [Tooltip("소형탄 범위(각도)")]
    public float sk1Angle;


    //대형탄관련
    [Header("Skill_1(대형탄)"), Space]

    [Tooltip("대형탄 데미지")]
    public float sk2Dmg;
    [Tooltip("기본 공격 소모량")]
    public int sk2Cost;
    [Tooltip("대형탄 쿨타임")]
    public float sk2CoolDown;
    [Tooltip("대형탄 사거리")]
    public float sk2Range;
    [Tooltip("대형탄 파괴 데미지")]
    public float sk2DestroyDmg;
    [Tooltip("대형탄 스플래쉬 범위")]
    public float sk2SplashRange;
    [Tooltip("대형탄 스플래쉬 데미지")]
    public float sk2SplashDmg;
    [Tooltip("대형탄 충전횟수")]
    public float sk2Count;
    

    //궁극기관련
    [Header("ultimate"), Space]

    [Tooltip("궁극기 충전량(%단위) // 20% 기준 - 충전값(10) + 20%(2))")]
    public int ultRestore;
    [Tooltip("궁극기 최대치")]
    public int UltMax;

}
