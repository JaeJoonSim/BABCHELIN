using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDotScript : BaseMonoBehaviour
{
    public float damage = 1f;
    public float destroyTime;

    public HealthPlayer player;
    private ColliderEvents colliderEvents;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        colliderEvents = GetComponent<ColliderEvents>();
        player = GameObject.FindObjectOfType<HealthPlayer>();

        colliderEvents.OnTriggerStayEvent += OnHit;

        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHit(Collider2D collider)
    {
        time += Time.deltaTime;

        if(time >= 0.5f)
        {
            time = 0;
            if (collider.tag == "Player")
            {
                Debug.Log("Dot hit " + collider.name);
                player.Damaged(gameObject, transform.position, damage, Health.AttackType.Poison);
            }
        }
        //else if (collider.tag == "Player")
        //{

        //}
    }
}
