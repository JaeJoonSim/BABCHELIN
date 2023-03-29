using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Popup : MonoBehaviour
{
    [Tooltip("Ű �Ҵ�")]
    [SerializeField]
    private KeyCode key = KeyCode.E;
    public KeyCode Key
    {
        get { return key; }
        set { key = value; }
    }

    [Tooltip("Ű �Ҵ�")]
    public GameObject PopupUI;

    private void Start()
    {
        PopupUI.SetActive(false);
    }

    private void Update()
    {
        ClosePopup();
    }

    private void ClosePopup()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PopupUI.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(key))
        {
            PopupUI.SetActive(true);
        }
    }

}
