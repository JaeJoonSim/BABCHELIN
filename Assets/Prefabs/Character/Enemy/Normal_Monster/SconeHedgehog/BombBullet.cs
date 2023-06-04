using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBullet : UDRLBullet
{
    // Start is called before the first frame update

    [SerializeField] float blastRange;

    [SerializeField] Transform target;
    private float distanceToPlayer;

    void Start()
    {
        colliderEvents = GetComponent<ColliderEvents>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        player = target.GetComponent<HealthPlayer>();

        colliderEvents.OnTriggerEnterEvent += OnHit;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (transform.position.z >= 0)
        {
            if(distanceToPlayer <= blastRange)
                player.Damaged(gameObject, transform.position, damage, Health.AttackType.Normal);
            Destroy(gameObject);
        }
    }
}
