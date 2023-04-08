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
            Debug.Log("Ãæµ¹ Player");
            if (playerTrigger.enabled)
            {
                StartCoroutine(BlinkEffect());
                KnockBack(enemyCollider);
                StartCoroutine(DisablePlayerTriggerForOffDamagedTime());
            }
        }
    }

    private MeshRenderer meshRenderer;
    private Collider enemyCollider;

    public float offDamagedTime = 1.0f;
    public Collider playerTrigger;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyCollider = other;
            
        }
    }

    private void KnockBack(Collider other)
    {
        if (playerTrigger.enabled && other != null)
        {
            Vector3 directionVector = transform.position - other.transform.position;
            Vector3 hitDirection = directionVector.normalized;
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();

            StartCoroutine(ApplyKnockBack(hitDirection, enemy.KnockBackForce, enemy.KnockbackDuration));
        }
    }

    IEnumerator BlinkEffect()
    {
        float blinkInterval = 0.1f;
        int blinkCount = 3;

        for (int i = 0; i < blinkCount * 2; i++)
        {
            meshRenderer.enabled = !meshRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
        meshRenderer.enabled = true;
    }

    IEnumerator ApplyKnockBack(Vector3 hitDirection, float knockBackForce, float knockBackDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < knockBackDuration)
        {
            float deltaTime = Time.deltaTime;
            elapsedTime += deltaTime;

            Vector3 moveDirection = hitDirection * knockBackForce * deltaTime;
            moveDirection.y = 0f;
            transform.position += moveDirection;

            yield return null;
        }
    }

    IEnumerator DisablePlayerTriggerForOffDamagedTime()
    {
        playerTrigger.enabled = false;
        yield return new WaitForSeconds(offDamagedTime);
        playerTrigger.enabled = true;
    }
}
