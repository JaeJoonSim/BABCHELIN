using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerUIPopup : MonoBehaviour
{
    public GameObject UI;

    private void Start()
    {
        if (UI == null)
            UI = gameObject.transform.GetChild(0).gameObject;
        UI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Time.timeScale = 0;

            UI.SetActive(true);

            Destroy(gameObject);
        }
    }
}
