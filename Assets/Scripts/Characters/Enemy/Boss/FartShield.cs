using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class FartShield : BaseMonoBehaviour
{
    private Health health;
    private Skunk skunk;

    void Start()
    {
        health = GetComponent<Health>();
        skunk = GetComponentInParent<Skunk>();
        if (skunk == null)
            skunk = FindObjectOfType<Skunk>();
    }

    void Update()
    {
        if (health.CurrentHP() <= 0)
        {
            skunk.RemoveShield();
            Destroy(gameObject);
        }
    }
}
