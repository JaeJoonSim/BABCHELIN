using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    public Health player;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        player = GameObject.FindWithTag("Player").GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            player.Damaged(gameObject, transform.position, damage, Health.AttackType.Normal);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(absorb.Instance.Player.GetComponent<PlayerController>().buffControl(1008, true));
        }
    }
}
