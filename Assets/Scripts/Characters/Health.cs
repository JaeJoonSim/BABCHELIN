using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : BaseMonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    public int multipleHealthLine;
    public int HpLineAmount;

    protected StateMachine state;
    protected Rigidbody2D rb;

    [SerializeField] protected float recoveryTime = 2.0f;

    protected MeshRenderer meshRenderer;
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public bool untouchable = false;

    public delegate void HitAction(GameObject Attacker, Vector3 AttackLocation);
    public delegate void HealthEvent(GameObject attacker, Vector3 attackLocation, float damage);
    public delegate void DieAction();

    public event DieAction OnDie;
    public event HitAction OnHit;
    public event HealthEvent OnDamaged;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = GetComponent<StateMachine>();
        currentHealth = maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        OnDamaged += ApplyChangeToHitState;
    }

    private void Update()
    {
        multipleHealthLine = (int)(maxHealth / HpLineAmount);
    }

    public void Damaged(GameObject Attacker, Vector3 attackLocation, float damage)
    {
        if (IsInvincible()) return;
        if (Attacker == base.gameObject) return;

        currentHealth -= damage;

        if (OnDamaged != null)
        {
            OnDamaged?.Invoke(gameObject, attackLocation, damage);
        }

        if (OnHit != null)
        {
            OnHit?.Invoke(Attacker, attackLocation);
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
        untouchable = true;
        state.CURRENT_STATE = StateMachine.State.Dead;
        OnDie?.Invoke();
    }

    protected IEnumerator InvincibilityAndBlink(float duration)
    {
        isInvincible = true;
        float blinkInterval = 0.1f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            meshRenderer.enabled = !meshRenderer.enabled;
            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        meshRenderer.enabled = true;
        isInvincible = false;

        if(currentHealth > 0)
            state.ChangeToIdleState();
    }

    protected virtual void ApplyChangeToHitState(GameObject attacker, Vector3 attackLocation, float damage)
    {
        StartCoroutine(InvincibilityAndBlink(recoveryTime));
        state.ChangeToHitState(attackLocation);
    }

    public float MaxHP()
    {
        return this.maxHealth;
    }

    public float CurrentHP()
    {
        return this.currentHealth;
    }
}