using System.Collections;
using UnityEngine;

public class HealthPlayer : Health
{
    protected override void Start()
    {
        base.Start();
        OnDamaged.AddListener(ApplyKnockbackAndChangeState);
    }

    private void ApplyKnockbackAndChangeState(float damage, Vector3 attackLocation)
    {
        Vector3 knockbackDirection = (transform.position - attackLocation).normalized;

        // �˹� ����, �ʿ� ������Ʈ Rigidbody2D
        // ...
        
        StartCoroutine(InvincibilityAndBlink(recoveryTime));
    }

    protected override bool IsInvincible()
    {
        return isInvincible || state.CURRENT_STATE == StateMachine.State.Dodging;
    }

    protected override void Die()
    {
        base.Die();
        // �÷��̾� ��� ���� ����
    }
}