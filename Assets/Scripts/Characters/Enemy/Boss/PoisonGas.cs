using System.Collections;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public float radius;
    public float speed;
    public float damage;
    public float duration;

    private void Start()
    {
        StartCoroutine(SpreadAndDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthPlayer>().isPoisoned = true;
        }
    }

    private IEnumerator SpreadAndDestroy()
    {
        float startTime = Time.time;
        while (transform.localScale.x < radius)
        {
            transform.localScale += new Vector3(1, 1, 0) * speed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
