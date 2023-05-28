using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TomatoBulletScript : BaseMonoBehaviour
{
    public float damage = 3f;
    public float speed = 10f;
    public HealthPlayer player;
    public float destroyTime = 5f;

    private ColliderEvents colliderEvents;
    private Vector3 direction;
    
    public float gravity;
    private float time;

    //public GameObject PlayerEffect;

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
