using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    public int CurrentHp
    {
        get { return currentHp; }
        set
        {
            currentHp = value;
            StartCoroutine(BlinkEffect());
        }
    }

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            KnockBack(other);
        }
    }

    private void KnockBack(Collider other)
    {
        Vector3 directionVector = transform.position  - other.transform.position;
        Vector3 hitDirection = directionVector.normalized;
        Enemy enemy;
        enemy = other.gameObject.GetComponentInParent<Enemy>();
        rb.AddForce(hitDirection * enemy.KnockBackForce, ForceMode.VelocityChange);

        playerMovement.StartCoroutine(playerMovement.KnockedBack(enemy.KnockbackDuration));
    }

    IEnumerator BlinkEffect()
    {
        float blinkInterval = 0.1f;
        int blinkCount = 5;
        for (int i = 0; i < blinkCount * 2; i++)
        {
            meshRenderer.enabled = !meshRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
        meshRenderer.enabled = true;
    }
}
