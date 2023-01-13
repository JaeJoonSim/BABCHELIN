using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTester : MonoBehaviour
{
    public bool showTesterPanel;
    public GameObject testPanel;
    public Inventory inventory;
    public ItemData[] itemDataArray;

    [Space(12)]
    public Button testButton1;
    public Button testButton2;
    public Button testButton3;

    private void Start()
    {
        testButton1.onClick.AddListener(() => { inventory.Add(itemDataArray[0]); });
        testButton2.onClick.AddListener(() => { inventory.Add(itemDataArray[1]); });
        testButton3.onClick.AddListener(() => { inventory.Add(itemDataArray[2]); });
    }

    private void Update()
    {
        if (showTesterPanel)
        {
            testPanel.SetActive(true);
        }
        else
        {
            testPanel.SetActive(false);
        }
    }
}
