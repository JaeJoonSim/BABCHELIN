using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public int num;

    [SerializeField] float blastRange;
    [SerializeField] float damage = 3f;
    [SerializeField] float speed = 10f;
    [SerializeField] float power = 5f;
    public HealthPlayer player;
    public float destroyTime = 5f;

    [SerializeField] Transform target;
    private ColliderEvents colliderEvents;
    private Vector3 direction;
    private float distanceToPlayer;

    public float gravity;
    private float time;

    void Start()
    {
        colliderEvents = GetComponent<ColliderEvents>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (player == null)
            player = target.GetComponent<HealthPlayer>();

        colliderEvents.OnTriggerEnterEvent += OnHit;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        distanceToPlayer = Vector3.Distance(transform.position, target.position);

        switch (num)
        {
            case 0:
                direction = new Vector3(0, 1, (speed * Mathf.Sin(90f * Mathf.Deg2Rad) * time + 0.5f * gravity * time) / 50f);
                break;
            case 1:
                direction = new Vector3(1, 0, (speed * Mathf.Sin(90f * Mathf.Deg2Rad) * time + 0.5f * gravity * time) / 50f);
                break;
            case 2:
                direction = new Vector3(0, -1, (speed * Mathf.Sin(90f * Mathf.Deg2Rad) * time + 0.5f * gravity * time) / 50f);
                break;
            case 3:
                direction = new Vector3(-1, 0, (speed * Mathf.Sin(90f * Mathf.Deg2Rad) * time + 0.5f * gravity * time) / 50f);
                break;
        }

        transform.Translate(direction * power * Time.deltaTime, Space.World);

        if (transform.position.z >= 0)
        {
            if(distanceToPlayer <= blastRange)
                player.Damaged(gameObject, transform.position, damage, Health.AttackType.Normal);
            Destroy(gameObject);
        }
    }

    public void OnHit(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            Debug.Log("Bullet hit " + collider.name);
            player.Damaged(gameObject, transform.position, damage, Health.AttackType.Normal);

            Destroy(gameObject);
        }
        else if (collider.tag == "Wall")
        {

        }
    }
}
