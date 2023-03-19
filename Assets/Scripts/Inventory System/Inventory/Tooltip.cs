using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [Tooltip("아이템 이름 텍스트")]
    [SerializeField]
    private LocalizedText itemName;
    public LocalizedText ItemName 
    { 
        get { return itemName; }
        set { itemName = value; }
    }

    [Tooltip("아이템 설명 텍스트")]
    [SerializeField]
    private LocalizedText itemDescription;
    public LocalizedText ItemDescription
    {
        get { return itemDescription; }
        set { itemDescription = value; }
    }

    private float halfWidth;
    private RectTransform rt;

    private void Start()
    {
        halfWidth = GetComponentInParent<CanvasScaler>().referenceResolution.x * 0.5f;
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        transform.position = Input.mousePosition;

        if (rt.anchoredPosition.x + rt.sizeDelta.x > halfWidth)
            rt.pivot = new Vector2(1, 1);
        else
            rt.pivot = new Vector2(0, 1);
    }

    public void SetupTooltip(string name, string description)
    {
        itemName.LocalizationKey = name;
        itemDescription.LocalizationKey = description;
    }
}
