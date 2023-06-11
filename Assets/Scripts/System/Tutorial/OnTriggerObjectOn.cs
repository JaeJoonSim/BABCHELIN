using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerObjectOn : MonoBehaviour
{
    public GameObject OnTriggerEnter;

    public PlayerController player;

    void Start()
    {
        OnTriggerEnter.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerController>();
            player.xDir = 0;
            player.yDir = 0;

            OnTriggerEnter.SetActive(true);

            //Destroy(gameObject);
        }
    }
}
