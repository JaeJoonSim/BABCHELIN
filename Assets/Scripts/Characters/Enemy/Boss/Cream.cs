using System;
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

    [Header("Landing Indicator Parameters")]
    public GameObject landingIndicatorPrefab;
    public float indicatorRadius = 1f;
    private GameObject landingIndicator;
    public float yOffset = 0.1f;

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

        if (landingIndicatorPrefab != null)
        {
            landingIndicator = Instantiate(landingIndicatorPrefab);
            landingIndicator.SetActive(false);
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

        UpdateLandingIndicator();
    }

    private void UpdateLandingIndicator()
    {
        if (landingIndicator != null)
        {
            if (transform.position.z < 0f)
            {
                landingIndicator.SetActive(true);

                Vector3 landingPosition = new Vector3(transform.position.x, transform.position.y, 0);
                landingIndicator.transform.position = landingPosition + new Vector3(0, yOffset, 0);
                landingIndicator.transform.localScale = new Vector3(indicatorRadius, indicatorRadius, 1f);

                float redCircleFillAmount = Mathf.Clamp01(1 - (transform.position.z / -10f));

                // Add the following code to update the child sprite scale
                Transform childSprite = landingIndicator.transform.GetChild(0); // Assumes the sprite is the first child
                if (childSprite != null)
                {
                    float distanceToGround = Mathf.Abs(transform.position.z);
                    float spriteScale = Mathf.Clamp01(1 - (distanceToGround / 10f)); // Adjust the divisor (10f) to change the distance at which the sprite scale becomes 0
                    childSprite.localScale = new Vector3(spriteScale, spriteScale, 1f);
                }
            }
            else
            {
                Destroy(landingIndicator);
            }
        }
    }

}
