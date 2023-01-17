using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [Tooltip("아이템 이름 텍스트")]
    [SerializeField]
    private TextMeshProUGUI itemName;
    public TextMeshProUGUI ItemName 
    { 
        get { return itemName; }
        set { itemName = value; }
    }

    [Tooltip("아이템 설명 텍스트")]
    [SerializeField]
    private TextMeshProUGUI itemDescription;
    public TextMeshProUGUI ItemDescription
    {
        get { return itemDescription; }
        set { itemDescription = value; }
    }

    public void SetupTooltip(string name, string description)
    {
        ItemName.text = name;
        ItemDescription.text = description;
    }
}
