using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDRLBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public int num;

    private AudioSource audioSource;
    public AudioClip popSound;

    [Space]
    public float damage;
    public float speed;
    public float range;

    public HealthPlayer player;

    private Vector3 startPosition;
    private float distanceRange;

    private ColliderEvents colliderEvents;
    private Vector3 direction;

    public float gravity;
    private float time;

    public GameObject PlayerEffect;
    public GameObject GroundEffect;

    protected virtual void Start()
    {
        colliderEvents = GetComponent<ColliderEvents>();
        if (player == null)
            player = GameObject.FindObjectOfType<HealthPlayer>();

        audioSource = GetComponent<AudioSource>();

        startPosition = transform.position;

        colliderEvents.OnTriggerEnterEvent += OnHit;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        distanceRange = Vector3.Distance(startPosition, transform.position);
        time += Time.deltaTime;

        switch (num)
        {
            case 0: //��
                if (distanceRange <= range)
                {
                    direction = new Vector3(0, 1, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(0, 1, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
            case 1: //������
                if (distanceRange <= range)
                {
                    direction = new Vector3(1, 0, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(1, 0, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
            case 2: //�Ʒ�
                if (distanceRange <= range)
                {
                    direction = new Vector3(0, -1, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(0, -1, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
            case 3: //����
                if (distanceRange <= range)
                {
                    direction = new Vector3(-1, 0, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(-1, 0, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
            case 4: //�ϼ�
                if (distanceRange <= range)
                {
                    direction = new Vector3(-0.6f, 0.6f, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(-0.6f, 0.6f, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
            case 5: //�ϵ�
                if (distanceRange <= range)
                {
                    direction = new Vector3(0.6f, 0.6f, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(0.6f, 0.6f, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
            case 6: //����
                if (distanceRange <= range)
                {
                    direction = new Vector3(-0.6f, -0.6f, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(-0.6f, -0.6f, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
            case 7: //����
                if (distanceRange <= range)
                {
                    direction = new Vector3(0.6f, -0.6f, 0);
                }
                else
                {
                    time += Time.deltaTime;
                    direction = new Vector3(0.6f, -0.6f, (Mathf.Sin(90f * Mathf.Deg2Rad) + gravity) * time / 5);
                }
                break;
        }
        transform.Translate(direction * speed * Time.deltaTime, Space.World);


        if (transform.position.z >= 0)
        {
            GameObject groundEffect = GroundEffect;
            groundEffect.transform.position = transform.position;
            Instantiate(groundEffect);

            if (popSound != null)
            {
                audioSource.clip = popSound;
                audioSource.PlayOneShot(audioSource.clip);
            }

            Destroy(gameObject);
        }
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

            if (popSound != null)
            {
                audioSource.clip = popSound;
                audioSource.PlayOneShot(audioSource.clip);
            }

            Destroy(gameObject);
        }
        else if (collider.tag == "Wall")
        {
            Debug.Log("Bullet hit " + collider.name);
            GroundEffect.transform.position = transform.position;
            GameObject groundEffect = Instantiate(GroundEffect);

            Destroy(gameObject);
        }
        else if (collider.tag == "DestroyableObject ")
        {
            Debug.Log("Bullet hit " + collider.name);
            GroundEffect.transform.position = transform.position;
            GameObject groundEffect = Instantiate(GroundEffect);

            Destroy(gameObject);
        }
    }
}
