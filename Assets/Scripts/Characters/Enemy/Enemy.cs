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
    [SerializeField] private float knockBackForce;
    [SerializeField] private float knockbackDuration = 0.5f;

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
            KnockBack(other);
        }
    }

    private void KnockBack(Collider other)
    {
        Vector3 directionVector = other.transform.position - transform.position;
        Vector3 hitDirection = directionVector.normalized;
        PlayerMovement playerMovement;
        playerMovement = other.gameObject.GetComponentInParent<PlayerMovement>();
        playerMovement.Rigid.AddForce(hitDirection * knockBackForce, ForceMode.VelocityChange);
        
        playerMovement.StartCoroutine(playerMovement.KnockedBack(knockbackDuration));
    }
}
