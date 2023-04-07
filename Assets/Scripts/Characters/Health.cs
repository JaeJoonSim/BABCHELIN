using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : BaseMonoBehaviour
{
    #region enum
    public enum AttackTypes
    {
        Melee = 0,
        Heavy = 1,
        Projectile = 2
    }

    [Flags]
    public enum AttackFlags
    {
        Crit = 1,
        Skull = 2,
        Poison = 4,
        Ice = 8,
        Charm = 0x10
    }
    #endregion

    #region delegate
    public delegate void DieAction(GameObject Attacker, Vector3 AttackLocation, Health Victim, AttackTypes AttackType, AttackFlags AttackFlags);
    public delegate void DieAllAction(Health Victim);
    public delegate void HitAction(GameObject Attacker, Vector3 AttackLocation, AttackTypes AttackType, bool FromBehind = false);
    public delegate void HealthEvent(GameObject attacker, Vector3 attackLocation, float damage);
    public delegate void StasisEvent();
    #endregion

    #region Class
    public class DealDamageEvent
    {
        public float UnscaledTimestamp;
        public float Damage;
        public GameObject Attacker;
        public Vector3 AttackLocation;
        public bool BreakBlocking;
        public AttackTypes AttackType;

        public DealDamageEvent(float unscaledTimestamp, float damage, GameObject attacker, Vector3 attackLocation, bool breakBlocking, AttackTypes attackType)
        {
            UnscaledTimestamp = unscaledTimestamp;
            Damage = damage;
            Attacker = attacker;
            AttackLocation = attackLocation;
            BreakBlocking = breakBlocking;
            AttackType = attackType;
        }
    }
    #endregion

    private Vector3 Velocity;

    [HideInInspector] public StateMachine state;

    [SerializeField] public float _totalHP = 1f;
    [SerializeField] public float _HP;

    public bool isPlayer;

    public virtual float totalHP
    {
        get { return _totalHP; }
        set { _totalHP = value; }
    }

    public virtual float HP
    {
        get { return _HP; }
        set { _HP = value; }
    }

    #region event
    public UnityEvent OnHitCallback;
    public UnityEvent OnDieCallback;

    public event DieAction OnDie;
    public static event DieAllAction OnDieAny;
    public event HitAction OnHit;
    public event HitAction OnHitEarly;
    public event HealthEvent OnDamaged;
    public event StasisEvent OnStasisCleared;
    #endregion

    #region List
    public static List<Health> allUnits = new List<Health>();
    public static List<Health> killAll = new List<Health>();
    #endregion

    private void Awake()
    {
        InitHP();
    }

    public virtual void InitHP()
    {
        if(isPlayer)
        {
            return;
        }

        HP = totalHP;
    }

    public virtual void OnEnable()
    {
        if(!allUnits.Contains(this))
        {
            allUnits.Add(this);
        }
        state = GetComponent<StateMachine>();
    }

    protected virtual void OnDisable()
    {
        if(isPlayer)
        {
            // AudioManager.Instance.StopLoop(value);
        }
        if(killAll.Contains(this)) 
        {
            killAll.Remove(this);
        }
        allUnits.Remove(this);
    }

    // Late DO...
    public static void DamageAllEnemies(float damage)
    {

    }

    //public virtual bool DealDamage(float Damage, GameObject Attacker, Vector3 AttackLocation, bool BreakBlocking = false, AttackTypes AttackType = AttackTypes.Melee, bool dealDamageImmediately = false, AttackFlags AttackFlags = (AttackFlags)0)
    //{
    //    if(!base.enabled)
    //    {
    //        return false;
    //    }
    //    if(state != null && dealDamageImmediately && (state.CURRENT_STATE == StateMachine.State.Dodging || state.CURRENT_STATE == StateMachine.State.InActive))
    //    {
    //        return false;
    //    }
    //    if(isPlayer && (state.CURRENT_STATE == StateMachine.State.CustomAnimation))
    //    {
    //        return false;
    //    }
    //    if(Attacker != null)
    //    {
    //        Velocity = AttackLocation - Attacker.transform.position;
    //    }
        
    //}
}
