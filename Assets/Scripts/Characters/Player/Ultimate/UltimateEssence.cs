using DebuggingEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateEssence : MonoBehaviour
{
    private void Start()
    {
        transform.position = transform.position + new Vector3(0, 0, -1f);
    }
    private void Update()
    {
        Vector3 direction = (absorb.Instance.Player.position + new Vector3(0,0,-0.5f)) - transform.position;

        // 정규화(normalize)된 방향 벡터를 사용하여 이동합니다.
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
