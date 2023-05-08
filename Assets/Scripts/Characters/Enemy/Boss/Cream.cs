using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cream : MonoBehaviour
{
    [SerializeField] private Collider2D bombCollider;

    [Header("Falling Parameters")]
    public float gravity = 9.81f;
    private float fallSpeed = 0f;
    private Vector3 fallDirection = new Vector3(0, 0, 1);

    private void Start()
    {
        if (bombCollider == null)
        {
            bombCollider = GetComponent<Collider2D>();
        }

        if (bombCollider != null)
        {
            bombCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (!bombCollider.enabled && transform.position.z >= 0f)
        {
            bombCollider.enabled = true;
        }

        if (transform.position.z < 0f)
        {
            fallSpeed += gravity * Time.deltaTime;
            transform.position += fallDirection * fallSpeed * Time.deltaTime;
        }
        else
        {
            fallSpeed = 0f;
        }
    }
}
