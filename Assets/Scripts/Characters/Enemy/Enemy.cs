using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    public int CurrentHp
    {
        get { return currentHp; }
        set
        {
            currentHp = value;
            Instantiate(damageText, damageTextCanvas);
        }
    }

    [Tooltip("Damage Text")]
    [SerializeField] private GameObject damageText;
    [Tooltip("Damage Text Canvas")]
    [SerializeField] private Transform damageTextCanvas;

    [HideInInspector] public int hitDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player is in range " + other.gameObject.name);
        }
    }
}
