using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : BaseMonoBehaviour
{
    public enum AttackType
    {
        Normal,
        Poison,

    }

    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float backHealth;
    public int multipleHealthLine;
    public int HpLineAmount;
    public bool isPoisoned = false;
    public bool IsPoisoned
    {
        get { return isPoisoned; }
        set { isPoisoned = value; }
    }

    protected StateMachine state;
    protected Rigidbody2D rb;
    protected DamageTextControler dtc;

    [SerializeField] protected float recoveryTime = 2.0f;

    protected MeshRenderer meshRenderer;
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public bool untouchable = false;
    [HideInInspector] public bool damageDecrease = false;
    [HideInInspector] public bool doNotChange = false;

    [SerializeField] private bool isMonster;

    public delegate void HitAction(GameObject Attacker, Vector3 AttackLocation, AttackType type);
    public delegate void HealthEvent(GameObject attacker, Vector3 attackLocation, float damage, AttackType type);
    public delegate void DieAction();

    public event DieAction OnDie;
    public event HitAction OnHit;
    public event HealthEvent OnDamaged;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = GetComponent<StateMachine>();
        dtc = GetComponent<DamageTextControler>();
        currentHealth = maxHealth;
        backHealth = currentHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        OnDamaged += ApplyChangeToHitState;

        if (HpLineAmount <= 0)
            HpLineAmount = 1;
    }

    private void Update()
    {
        multipleHealthLine = (int)(currentHealth / HpLineAmount);
    }

    public void Damaged(GameObject Attacker, Vector3 attackLocation, float damage, AttackType type, int destructionCount = 0)
    {
        if (IsInvincible())
        {
            dtc.ShowDamageText(0);
            return;
        }
        //if (Attacker == base.gameObject) return;

        if (destructionCount > 0)
        {
            damage = damage - (damage * (destructionCount * 0.1f));
        }

        if (damageDecrease)
        {
            damage *= 0.01f;
        }

        currentHealth -= damage;
        Invoke("DecreaseBackHP", 0.3f);

        if(isMonster)
        {
            dtc.ShowDamageText(damage);
        }

        if (OnDamaged != null)
        {
            OnDamaged?.Invoke(gameObject, attackLocation, damage, type);
        }

        if (OnHit != null)
        {
            OnHit?.Invoke(Attacker, attackLocation, type);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual bool IsInvincible()
    {
        return untouchable || isInvincible;
    }

    protected virtual void Die()
    {
        // 사망 로직 구현
        //Debug.Log($"{base.gameObject.name} Dead");
        state.LockStateChanges = false;
        untouchable = true;
        state.CURRENT_STATE = StateMachine.State.Dead;
        OnDie?.Invoke();
    }

    protected virtual IEnumerator InvincibilityAndBlink(float duration)
    {
        isInvincible = true;
        float blinkInterval = 0.1f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            if (meshRenderer != null)
                meshRenderer.enabled = !meshRenderer.enabled;
            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        if (meshRenderer != null)
            meshRenderer.enabled = true;
        isInvincible = false;

        if (currentHealth > 0)
        {
            if (isMonster)
            {
                state.ChangeToPreviousState();
            }
            else
            {
                if (!damageDecrease && !doNotChange)
                    state.ChangeToIdleState();
            }
        }
    }

    protected virtual void ApplyChangeToHitState(GameObject attacker, Vector3 attackLocation, float damage, AttackType type)
    {
        if (type == AttackType.Normal)
        {
            StartCoroutine(InvincibilityAndBlink(recoveryTime));
            if (!damageDecrease && !doNotChange)
                state.ChangeToHitState(attackLocation);
        }
    }

    public float MaxHP()
    {
        return this.maxHealth;
    }

    public float CurrentHP()
    {
        return this.currentHealth;
    }

    private void DecreaseBackHP()
    {
        backHealth = currentHealth;
    }

    public float BackHP()
    {
        return this.backHealth;
    }
}