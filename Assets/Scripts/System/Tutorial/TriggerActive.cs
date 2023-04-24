using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActive : BaseMonoBehaviour
{
    public GameObject video;
    private BoxCollider2D boxCollider2D;

    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            video.SetActive(true);
            boxCollider2D.enabled = false;
        }
    }
}
