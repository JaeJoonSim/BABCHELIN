using DebuggingEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateEssence : MonoBehaviour
{
    private void Update()
    {
        Vector3 direction = absorb.Instance.Player.position - transform.position;

        // ����ȭ(normalize)�� ���� ���͸� ����Ͽ� �̵��մϴ�.
        transform.Translate(direction.normalized * absorb.Instance.speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().addUltGauge(5);
            Destroy(gameObject);
        }
    }
}
