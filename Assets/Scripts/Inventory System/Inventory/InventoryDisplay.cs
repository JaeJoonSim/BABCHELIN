using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine;

public class InventoryDisplay : MonoBehaviour
{
    [Tooltip("�κ��丮 ������")]
    [SerializeField]
    private GameObject inventoryPrefab;
    public GameObject InventoryPrefab
    {
        get { return inventoryPrefab; }
        set { inventoryPrefab = value; }
    }

    [Tooltip("�κ��丮")]
    [SerializeField]
    private Inventory inventory;
    public Inventory Inventory { get { return inventory; } }

    [Tooltip("�����۰� �¿� �Ÿ�")]
    [SerializeField]
    private float xSpaceBetweenItem;
    public float XSpaceBetweenItem
    {
        get { return xSpaceBetweenItem; }
        set { xSpaceBetweenItem = value; }
    }

    [Tooltip("�����۰� ���� �Ÿ�")]
    [SerializeField]
    private float ySpaceBetweenItem;
    public float YSpaceBetweenItem
    {
        get { return ySpaceBetweenItem; }
        set { ySpaceBetweenItem = value; }
    }

    [Tooltip("�� ����")]
    [SerializeField]
    private int numberOfColumn;
    public int NumberOfColumn
    {
        get { return numberOfColumn; }
        set { numberOfColumn = value; }
    }

    [Tooltip("X ���� ��ǥ")]
    [SerializeField]
    private float xStart;
    public float XStart
    {
        get { return xStart; }
        set { xStart = value; }
    }

    [Tooltip("Y ���� ��ǥ")]
    [SerializeField]
    private float yStart;
    public float YStart
    {
        get { return yStart; }
        set { yStart = value; }
    }

    Dictionary<InventorySlot, GameObject> itemsDisplay = new Dictionary<InventorySlot, GameObject>();

    private void Start()
    {
        CreateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < inventory.Items.Items.Count; i++)
        {
            InventorySlot slot = inventory.Items.Items[i];

            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.ItemDatabase.GetItem[slot.Item.ID].UiDisplay;
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.Amount.ToString("n0");
            itemsDisplay.Add(slot, obj);
        }
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Items.Items.Count; i++)
        {
            InventorySlot slot = inventory.Items.Items[i];

            if (itemsDisplay.ContainsKey(inventory.Items.Items[i]))
            {
                itemsDisplay[inventory.Items.Items[i]].GetComponentInChildren<TextMeshProUGUI>().text = slot.Amount.ToString("n0");
            }
            else
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.ItemDatabase.GetItem[slot.Item.ID].UiDisplay;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.Amount.ToString("n0");
                itemsDisplay.Add(slot, obj);
            }
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(xStart + (xSpaceBetweenItem * (i % numberOfColumn)), yStart + (-ySpaceBetweenItem * (i / numberOfColumn)), 0f);
    }
}
