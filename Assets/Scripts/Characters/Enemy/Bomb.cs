using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BaseMonoBehaviour
{
    [SerializeField] Transform target;
    private Health playerHealth;

    [SerializeField] private float blastRange;
    public float Damaged = 2f;

    [SerializeField] private float timer = 5f;

    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        BombExplosion();
    }

    private void BombExplosion()
    {
        timer -= Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (timer <= 0f)
        {
            if(distanceToPlayer <= blastRange)
            {
                playerHealth.Damaged(gameObject, transform.position, Damaged);
            }

            Destroy(this.gameObject);
        }

    }
}