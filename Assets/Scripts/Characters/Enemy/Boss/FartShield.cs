using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class FartShield : BaseMonoBehaviour
{
    public float shieldDamage = 2f;
    public int touchPlayerCount = 3;

    private Health health;
    private Skunk skunk;

    void Start()
    {
        health = GetComponent<Health>();
        skunk = GetComponentInParent<Skunk>();
        if (skunk == null)
            skunk = FindObjectOfType<Skunk>();
    }

    void Update()
    {
        if (health.CurrentHP() <= 0)
        {
            skunk.RemoveShield();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthPlayer player = collision.GetComponent<HealthPlayer>();
            if (player != null)
            {
                player.Damaged(gameObject, transform.position, shieldDamage, Health.AttackType.Normal);
                player.fartShieldCount++;
                if(player.fartShieldCount >= touchPlayerCount)
                {
                    player.isPoisoned = true;
                    player.fartShieldCount = 0;
                }
            }
        }
    }
}
