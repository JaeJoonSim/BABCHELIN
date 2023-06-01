using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerUIPopup : MonoBehaviour
{
    private GameObject UI;

    private void Start()
    {
        UI = gameObject.transform.GetChild(0).gameObject;
        UI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Time.timeScale = 0;

            UI.SetActive(true);

            enabled = false;
        }
    }
}
