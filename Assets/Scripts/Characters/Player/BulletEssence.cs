using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEssence : MonoBehaviour
{
    int addValue;

    private void Start()
    {
        transform.position = transform.position + new Vector3(0, 0, -1f);
    }
    private void Update()
    {
        Vector3 direction = (absorb.Instance.Player.position + new Vector3(0, 0, -0.5f)) - transform.position;

        // ����ȭ(normalize)�� ���� ���͸� ����Ͽ� �̵��մϴ�.
        transform.Translate(direction.normalized * absorb.Instance.speed * Time.deltaTime);
    }

    public void setAddValue(int value)
    {
        addValue = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().addBullet(addValue);
            Destroy(gameObject);

            Cream parent = gameObject.GetComponentInParent<Cream>();
            if (parent != null)
                Destroy(parent);
        }
    }
}
