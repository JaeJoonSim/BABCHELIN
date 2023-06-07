using System.Collections;
using UnityEngine;

public class FartProjectile : MonoBehaviour
{
    [Tooltip("스폰시 이동 속도")]
    [SerializeField] private float spawnSpeed = 1f;
    [Tooltip("공격 감지 범위")]
    [SerializeField] private float radius = 1f;
    [Tooltip("스폰 이후 이동 속도")]
    [SerializeField] private float speed = 1f;
    [Tooltip("스폰 후, 몇초 이후에 플레이어를 추적할 것인지?")]
    [SerializeField] private float duration = 1f;
    [Tooltip("데미지")]
    [SerializeField] private float damage = 1f;
    [Tooltip("중독 까지의 시간")]
    [SerializeField] private float poisoningDuration = 1f;
    [Tooltip("중독 데미지")]
    [SerializeField] private float poisoningDamage = 1f;
    [Tooltip("스폰 후, 몇초 이후에 파괴할 것인지?")]
    [SerializeField] private float lifetime = 5f;

    private Transform player;
    private HealthPlayer playerHealth;
    private Rigidbody2D rb;
    private float timer;
    private bool isPoisoned;
    private ParticleSystem fart;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<HealthPlayer>();
        rb = GetComponent<Rigidbody2D>();
        fart = GetComponentInChildren<ParticleSystem>();

        fart.Play(true);
        StartCoroutine(FartBehavior());
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
        else if (timer >= duration)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            rb.velocity = directionToPlayer * speed;
        }
    }

    private IEnumerator FartBehavior()
    {
        rb.velocity = -transform.up * spawnSpeed;
        yield return new WaitForSeconds(duration);

        while (true)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= radius)
            {
                playerHealth.curPoisonTimer = 0f;
                playerHealth.poisonDamage = poisoningDamage;

                if (isPoisoned == false)
                {
                    playerHealth.Damaged(gameObject, transform.position, damage, Health.AttackType.Poison);
                    isPoisoned = true;
                }
                playerHealth.IsPoisoned = true;

            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
