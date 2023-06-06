using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : BaseMonoBehaviour
{
    public HealthPlayer player;

    private AudioSource audioSource;
    public AudioClip popSound;

    [Space]
    public float damage;
    public float speed;
    public float range;

    private ColliderEvents colliderEvents;
    private Vector3 direction;
    private Vector3 startPosition;
    private float distanceRange;

    public GameObject PlayerEffect;
    public GameObject GroundEffect;

    private float gravity = 9.81f;
    private float time;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        colliderEvents = GetComponent<ColliderEvents>();
        player = GameObject.FindObjectOfType<HealthPlayer>();
        startPosition = transform.position;
        direction = (player.transform.position - transform.position).normalized;

        colliderEvents.OnTriggerEnterEvent += OnHit;

        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = 0;
        transform.eulerAngles = currentRotation;
    }

    protected virtual void Update()
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

        if(transform.position.z >= 0)
        {
            GroundEffect.transform.position = transform.position;
            GameObject groundEffect = Instantiate(GroundEffect);

            if(popSound != null)
            {
                audioSource.clip = popSound;
                audioSource.PlayOneShot(audioSource.clip);
            }

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

            audioSource.clip = popSound;
            audioSource.PlayOneShot(audioSource.clip);

            Destroy(gameObject);
        }
    }
}
