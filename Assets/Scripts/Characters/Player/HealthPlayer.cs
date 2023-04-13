using System.Collections;
using UnityEngine;

public class HealthPlayer : Health
{
    protected override void Start()
    {
        base.Start();
        OnDamaged += ApplyKnockbackAndChangeState;
    }

    private void ApplyKnockbackAndChangeState(GameObject Attacker, Vector3 attackLocation, float damage)
    {
        StartCoroutine(InvincibilityAndBlink(recoveryTime));
    }

    protected override bool IsInvincible()
    {
        return isInvincible || state.CURRENT_STATE == StateMachine.State.Dodging;
    }

    protected override void Die()
    {
        base.Die();
        // 플레이어 사망 로직 구현
    }
}