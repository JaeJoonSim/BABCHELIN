using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelControll : MonoBehaviour
{
    [Tooltip("키 할당")]
    [SerializeField]
    private KeyCode key = KeyCode.Tab;
    public KeyCode Key
    {
        get { return key; }
        set { key = value; }
    }

    [Tooltip("패널")]
    [SerializeField]
    private GameObject[] panel;

    private void Start()
    {
        for (int i = 0; i < panel.Length; i++)
        {
            panel[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(key) && Time.timeScale == 1)
        {
            Time.timeScale = 0;
            for (int i = 0; i < panel.Length; i++)
            {
                panel[i].SetActive(!panel[i].activeSelf);
            }
        }
        else if (Input.GetKeyDown(key) && Time.timeScale == 0 && panel[0].activeSelf == true)
        {
            for (int i = 0; i < panel.Length; i++)
            {
                panel[i].SetActive(!panel[i].activeSelf);
            }
            Time.timeScale = 1;
        }
    }
}
