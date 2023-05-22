using System.Collections;
using UnityEngine;

public class FartProjectile : MonoBehaviour
{
    [Tooltip("������ �̵� �ӵ�")]
    [SerializeField] private float spawnSpeed = 1f;
    [Tooltip("���� ���� ����")]
    [SerializeField] private float radius = 1f;
    [Tooltip("���� ���� �̵� �ӵ�")]
    [SerializeField] private float speed = 1f;
    [Tooltip("���� ��, ���� ���Ŀ� �÷��̾ ������ ������?")]
    [SerializeField] private float duration = 1f;
    [Tooltip("������")]
    [SerializeField] private float damage = 1f;
    [Tooltip("�ߵ� ������ �ð�")]
    [SerializeField] private float poisoningDuration = 1f;
    [Tooltip("�ߵ� ������")]
    [SerializeField] private float poisoningDamage = 1f;
    [Tooltip("���� ��, ���� ���Ŀ� �ı��� ������?")]
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
