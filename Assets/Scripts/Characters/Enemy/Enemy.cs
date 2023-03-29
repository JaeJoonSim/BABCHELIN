using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private int damage;

    [Space]
    [SerializeField] private float knockBackForce;
    public float KnockBackForce
    {
        get { return knockBackForce; }
    }
    [SerializeField] private float knockbackDuration = 0.5f;
    public float KnockbackDuration
    {
        get { return knockbackDuration; }
    }

    [Tooltip("Damage Text")]
    [SerializeField] private GameObject damageText;
    [Tooltip("Damage Text Canvas")]
    [SerializeField] private Transform damageTextCanvas;
    [Tooltip("HP Bar")]
    [SerializeField] private Image hpBar;

    [HideInInspector] public int hitDamage;

    private void Update()
    {
        hpBar.fillAmount = (float)currentHp / maxHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().CurrentHp -= damage;
        }
    }
}
