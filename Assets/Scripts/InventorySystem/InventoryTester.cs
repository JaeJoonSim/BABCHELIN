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

    private void Start()
    {
        testButton1.onClick.AddListener(() => { inventory.Add(itemDataArray[0]); });
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
