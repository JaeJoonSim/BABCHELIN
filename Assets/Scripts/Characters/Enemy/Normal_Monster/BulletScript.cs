using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : BaseMonoBehaviour
{
    public HealthPlayer player;

    [Space]
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float range;

    private ColliderEvents colliderEvents;
    private Vector3 direction;
    private Vector3 startPosition;
    private float distanceRange;

    [Space]
    public GameObject PlayerEffect;
    public GameObject GroundEffect;

    private float gravity = 9.81f;
    private float time;

    private void Start()
    {
        colliderEvents = GetComponent<ColliderEvents>();
        player = GameObject.FindObjectOfType<HealthPlayer>();
        startPosition = transform.position;
        direction = (player.transform.position - transform.position).normalized;

        colliderEvents.OnTriggerEnterEvent += OnHit;

        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = 0;
        transform.eulerAngles = currentRotation;
    }

    private void Update()
    {
        distanceRange = Vector3.Distance(startPosition, transform.position);

        if (distanceRange <= range)
        {
            direction = new Vector3(direction.x, direction.y, 0);
        }
        else
        {
            time += Time.deltaTime;
            direction = new Vector3(direction.x, direction.y, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
        }
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

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
