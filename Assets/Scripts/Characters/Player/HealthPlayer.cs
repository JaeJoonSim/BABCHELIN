using UnityEngine;

public class HealthPlayer : Health
{
    public delegate void HPUpdated(HealthPlayer Target);
    public delegate void TotalHPUpdated(HealthPlayer Target);

    public static bool ResetHealthData = true;

    public override float HP
    {
        get
        {
            return _HP;
        }
        set
        {
            Debug.Log($"HP CHANGE: {_HP} + {value}");
            if(_HP > _totalHP)
            {
                _HP = _totalHP;
            }
            if (HealthPlayer.OnHPUpdated != null)
			{
				HealthPlayer.OnHPUpdated(this);
			}
        }
    }

    public override float totalHP
    {
        get
        {
            return _totalHP;
        }
        set
        {
            Debug.Log($"Total CHANGE: {_totalHP} + {value}");
            _totalHP = value;
            if (HealthPlayer.OnTotalHPUpdated != null)
            {
                HealthPlayer.OnTotalHPUpdated(this);
            }
        }
    }

    public static event HPUpdated OnHPUpdated;
    public static event HPUpdated OnHeal;
    public new static event HPUpdated OnDamaged;
    public static event HPUpdated OnPlayerDied;
    public static event HPUpdated OnTotalHPUpdated;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void InitHP()
    {
        base.InitHP();

        if (ResetHealthData)
        {
            _HP = totalHP;
        }
        ResetHealthData = false;
        _HP = HP;
        _totalHP = totalHP;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        base.OnDie -= HealthPlayer_OnDie;
    }

    private void HealthPlayer_OnDie(GameObject Attacker, Vector3 AttackLocation, Health Victim, AttackTypes AttackType, AttackFlags AttackFlags)
    {
        HealthPlayer.OnPlayerDied?.Invoke(this);
    }

    public override bool DealDamage(float Damage, GameObject Attacker, Vector3 AttackLocation, bool BreakBlocking = false, AttackTypes AttackType = AttackTypes.Melee, bool dealDamageImmediately = false, AttackFlags AttackFlags = 0)
    {
        bool num = base.DealDamage(Damage, Attacker, AttackLocation, BreakBlocking, AttackType, dealDamageImmediately, AttackFlags);
        if (num) 
        {
            HPUpdated onDamaged = HealthPlayer.OnDamaged;
            if(onDamaged == null)
            {
                return num;
            }
            onDamaged(this);
        }
        return num;
    }


}
