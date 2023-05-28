using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public int num;

    public float damage = 3f;
    public float speed = 10f;
    public float power = 5f;
    public HealthPlayer player;
    public float destroyTime = 5f;

    private ColliderEvents colliderEvents;
    private Vector3 direction;

    public float gravity;
    private float time;

    void Start()
    {
        colliderEvents = GetComponent<ColliderEvents>();
        if (player == null)
            player = GameObject.FindObjectOfType<HealthPlayer>();

        colliderEvents.OnTriggerEnterEvent += OnHit;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

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
