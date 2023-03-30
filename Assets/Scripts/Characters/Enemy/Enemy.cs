using BehaviourTreeSystem;
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

    [Tooltip("Attack Collider")]
    [SerializeField] private Collider attackCollider;
    public Collider AttackCollider
    {
        get { return attackCollider; }
    }
    [Tooltip("Damage Text")]
    [SerializeField] private GameObject damageText;
    [Tooltip("Damage Text Canvas")]
    [SerializeField] private Transform damageTextCanvas;
    [Tooltip("HP Bar")]
    [SerializeField] private Image hpBar;

    [HideInInspector] public int hitDamage;

    private Animator anim;
    private BehaviourTreeRunner behaviourTree;
    private bool isDie = false;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        behaviourTree = GetComponent<BehaviourTreeRunner>();
        attackCollider.enabled = false;
    }

    private void Update()
    {
        hpBar.fillAmount = (float)currentHp / maxHp;

        if (currentHp <= 0 && !isDie)
        {
            Die();
        }
        CheckAnim();
    }

    private void Die()
    {
        isDie = true;
        behaviourTree.enabled = false;
        anim.SetBool("IsDead", true);
        Destroy(gameObject, 3.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().CurrentHp -= damage;
        }
    }
    
    private void CheckAnim()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f)
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }
    }
}
