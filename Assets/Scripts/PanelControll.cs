using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelControll : MonoBehaviour
{
    [Tooltip("Ű �Ҵ�")]
    [SerializeField]
    private KeyCode key = KeyCode.Tab;
    public KeyCode Key
    {
        get { return key; }
        set { key = value; }
    }

    [Tooltip("�г�")]
    [SerializeField]
    private GameObject[] panel;

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            for (int i = 0; i < panel.Length; i++)
            {
                panel[i].SetActive(!panel[i].activeSelf);
            }
        }
    }
}
