using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Countertop : MonoBehaviour
{
    public GameObject CountertopUI;

    private void Start()
    {
        CountertopUI.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            CountertopUI.SetActive(true);
        }
    }
}