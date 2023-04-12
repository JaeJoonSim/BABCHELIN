using System.Collections;
using UnityEngine;

public class HealthPlayer : Health
{
    public bool ScreenShakeOnHit = true;
    public float ShakeIntensity = 10f;

    protected override void Start()
    {
        base.Start();
        OnDamaged += ApplyKnockbackAndChangeState;
    }

    private void ApplyKnockbackAndChangeState(GameObject Attacker, Vector3 attackLocation, float damage)
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