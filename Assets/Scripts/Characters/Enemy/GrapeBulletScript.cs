using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrapeBulletScript : BaseMonoBehaviour
{
    public float damage = 2f;
    public float speed = 10f;
    public HealthPlayer player;
    public float destroyTime = 5f;

    private ColliderEvents colliderEvents;
    private Vector3 direction;

    public GameObject PlayerEffect;
    public GameObject GroundEffect;

    public float gravity = 1f;
    private float time;

    private void Start()
    {
        colliderEvents = GetComponent<ColliderEvents>();
        player = GameObject.FindObjectOfType<HealthPlayer>();
        direction = (player.transform.position - transform.position).normalized;

        colliderEvents.OnTriggerEnterEvent += OnHit;

        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = 0;
        transform.eulerAngles = currentRotation;
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        time += Time.deltaTime;
        direction = new Vector3(direction.x, direction.y, (speed * Mathf.Sin(90f * Mathf.Deg2Rad) * time + 0.5f * gravity * time) / 50f);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (transform.position.z >= 0)
        {
            GameObject groundEffect = GroundEffect;
            groundEffect.transform.position = transform.position;
            Instantiate(groundEffect);
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    public void OnHit(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            Debug.Log("Bullet hit " + collider.name);
            player.Damaged(gameObject, transform.position, damage, Health.AttackType.Normal);

            GameObject playerEffect = PlayerEffect;
            playerEffect.transform.position = transform.position;
            Instantiate(playerEffect);

            Destroy(gameObject);
        }
        else if (collider.tag == "Player")
        {

        }
    }
}
