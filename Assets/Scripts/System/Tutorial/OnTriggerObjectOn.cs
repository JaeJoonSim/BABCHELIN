using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerObjectOn : MonoBehaviour
{
    public GameObject OnTriggerEnter;
    
    void Start()
    {
        OnTriggerEnter.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            OnTriggerEnter.SetActive(true);

            Destroy(gameObject);
        }
    }
}
