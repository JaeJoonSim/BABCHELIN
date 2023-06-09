using System.Collections;
using UnityEngine;

public class HealthPlayer : Health
{
    [HideInInspector] public PlayerController controller;
    public float KnockbackForce = 2f;

    [SerializeField] private GameObject RetryPanel;

    [HideInInspector] public float poisoningDamage;
    public float clearPoisonTimer;
    public float curPoisonTimer;
    [HideInInspector] public float poisonDamage = 5f;
    public float poisonDamageInterval;
    public float poisonIntervalCounter;
    [HideInInspector] public int fartShieldCount = 0;

    protected override void Start()
    {
        base.Start();
        if(RetryPanel.activeSelf)
            RetryPanel.SetActive(false);

        if (poisonDamage == 0)
            poisonDamage = 5f;

        controller = GetComponent<PlayerController>();
        OnDamaged += ApplyKnockbackAndChangeState;

    }

    private void Update()
    {
        maxHealth = controller.TotalStatus.hpMax.value;
        if (isPoisoned == true)
        {
            curPoisonTimer += Time.deltaTime;
            if (curPoisonTimer > clearPoisonTimer)
            {
                isPoisoned = false;
                curPoisonTimer = 0;
            }
            else
            {
                PoisonDamaged();
            }
        }
    }

    private void ApplyKnockbackAndChangeState(GameObject Attacker, Vector3 attackLocation, float damage, AttackType type)
    {
        if(type == AttackType.Normal)
            StartCoroutine(ApplyKnockbackCoroutine(Attacker, attackLocation, recoveryTime));
    }

    private IEnumerator ApplyKnockbackCoroutine(GameObject attacker, Vector3 attackLocation, float knockbackDuration)
    {
        Vector3 knockbackDirection = transform.position - attacker.transform.position;
        knockbackDirection.z = 0;
        knockbackDirection.Normalize();
        controller.speed = KnockbackForce;

        CameraManager.instance.ShakeCameraForDuration(0.6f, 0.8f, 0.3f, StackShakes: false);

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

    private void PoisonDamaged()
    {
        poisonIntervalCounter += Time.deltaTime;
        if (poisonIntervalCounter >= poisonDamageInterval)
        {
            poisonIntervalCounter = 0;
            Damaged(gameObject, transform.position, poisonDamage, AttackType.Poison);
        }
    }

    private IEnumerator RetryPanelCoroutine()
    {
        yield return new WaitForSeconds(3f);
        RetryPanel.SetActive(true);
    }


}