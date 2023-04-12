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
    protected bool isInvincible = false;

    [Space]
    public UnityEvent<float, Vector3> OnDamaged;

    protected virtual void Start()
    {
        state = GetComponent<StateMachine>();
        currentHealth = maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        OnDamaged.AddListener(ApplyChangeToHitState);
    }

    public void Damaged(float damage, Vector3 attackLocation)
    {
        if (IsInvincible()) return;

        currentHealth -= damage;

        if (OnDamaged != null)
        {
            OnDamaged.Invoke(damage, attackLocation);
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
        // 사망 로직 구현
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

    protected virtual void ApplyChangeToHitState(float damage, Vector3 attackLocation)
    {
        state.ChangeToHitState(attackLocation);
    }
}