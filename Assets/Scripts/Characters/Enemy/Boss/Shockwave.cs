using UnityEngine;

public class Shockwave : BaseMonoBehaviour
{
    private float range;
    private float duration;
    private float startTime;

    public void Initialize(float range, float duration)
    {
        this.range = range;
        this.duration = duration;
        startTime = Time.time;
    }

    void Update()
    {
        float elapsed = Time.time - startTime;
        if (elapsed > duration)
        {
            Destroy(gameObject);
        }
        else
        {
            float scale = Mathf.Lerp(0f, range, elapsed / duration);
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
