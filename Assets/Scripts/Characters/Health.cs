using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    protected StateMachine state;

    [SerializeField] protected float recoveryTime = 2.0f;

    protected MeshRenderer meshRenderer;
    [HideInInspector] public bool isInvincible = false;

    public delegate void HitAction(GameObject Attacker, Vector3 AttackLocation);
    public delegate void HealthEvent(GameObject attacker, Vector3 attackLocation, float damage);

    public event HitAction OnHit;
    public event HealthEvent OnDamaged;

    protected virtual void Start()
    {
        state = GetComponent<StateMachine>();
        currentHealth = maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        OnDamaged += ApplyChangeToHitState;
    }

    public void Damaged(GameObject Attacker, Vector3 attackLocation, float damage)
    {
        Debug.Log("Hit Test");
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
        return false;
    }

    protected virtual void Die()
    {
        // ��� ���� ����
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

        state.ChangeToIdleState();
    }

    protected virtual void ApplyChangeToHitState(GameObject attacker, Vector3 attackLocation, float damage)
    {
        state.ChangeToHitState(attackLocation);
    }
}