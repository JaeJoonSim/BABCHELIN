using System.Collections;
using UnityEngine;

public class HealthPlayer : Health
{
    [HideInInspector] public PlayerController controller;
    public float KnockbackForce = 2f;

    [SerializeField] private GameObject RetryPanel;

    protected override void Start()
    {
        base.Start();
        if(RetryPanel.activeSelf)
            RetryPanel.SetActive(false);
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
        return untouchable || isInvincible || state.CURRENT_STATE == StateMachine.State.Dodging;
    }

    protected override void Die()
    {
        base.Die();
        controller.speed = 0;
        // 플레이어 사망 로직 구현

        StartCoroutine(RetryPanelCoroutine());
    }

    private IEnumerator RetryPanelCoroutine()
    {
        yield return new WaitForSeconds(3f);
        RetryPanel.SetActive(true);
    }
}