using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTab : MonoBehaviour
{
    int tabNum;
    public GameObject IventoryTab;
    public GameObject AccelTab;

    // Start is called before the first frame update
    void Start()
    {
        tabNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(tabNum == 0)
        {
            IventoryTab.SetActive(true);
            AccelTab.SetActive(false);
        }
        else if (tabNum == 1)
        {
            AccelTab.SetActive(true);
            IventoryTab.SetActive(false);
        }
    }

    public void OpenIventoryTab()
    {
        tabNum = 0;
    }

    public void OpenAccelTab()
    {
        tabNum = 1;
    }
}
