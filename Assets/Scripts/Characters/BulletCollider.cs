using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCollider : BaseMonoBehaviour
{
    public float damage = 2f;
    public float speed = 10f;
    public HealthPlayer player;
    public float destroyTime = 5f;

    private Vector2 direction;

    private void Start()
    {
        player = GameObject.FindObjectOfType<HealthPlayer>();
        direction = (player.transform.position - transform.position).normalized;

        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = 0;
        transform.eulerAngles = currentRotation;
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
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
            player.Damaged(gameObject, transform.position, damage);
            Destroy(gameObject);
        }
    }
}
