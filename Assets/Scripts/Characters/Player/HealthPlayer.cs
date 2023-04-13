using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class HealthPlayer : Health
{
    [HideInInspector] public PlayerController controller;
    public float KnockbackForce = 2f;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        OnDamaged += ApplyKnockbackAndChangeState;
    }

    private void ApplyKnockbackAndChangeState(GameObject Attacker, Vector3 attackLocation, float damage)
    {
        StartCoroutine(ApplyKnockbackCoroutine(Attacker, attackLocation, recoveryTime));
        //StartCoroutine(InvincibilityAndBlink(recoveryTime));
    }

    private IEnumerator ApplyKnockbackCoroutine(GameObject attacker, Vector3 attackLocation, float knockbackDuration)
    {
        Vector3 knockbackDirection = transform.position - attacker.transform.position;
        knockbackDirection.z = 0;
        knockbackDirection.Normalize();
        controller.speed = KnockbackForce;

        yield return new WaitForSeconds(knockbackDuration / 2);
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