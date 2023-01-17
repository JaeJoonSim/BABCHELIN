using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InventoryDisplay : MonoBehaviour
{
    [Tooltip("마우스")]
    [SerializeField]
    private MouseItem mouseItem = new MouseItem();
    public MouseItem MouseItem 
    { 
        get { return mouseItem; }
        set { mouseItem = value; }
    }

    [Tooltip("인벤토리 프리팹")]
    [SerializeField]
    private GameObject inventoryPrefab;
    public GameObject InventoryPrefab
    {
        get { return inventoryPrefab; }
        set { inventoryPrefab = value; }
    }

    [Tooltip("인벤토리")]
    [SerializeField]
    private Inventory inventory;
    public Inventory Inventory { get { return inventory; } }

    Dictionary<GameObject, InventorySlot> itemsDisplay = new Dictionary<GameObject, InventorySlot>();

    private void Start()
    {
        CreateSlots();
    }

    private void Update()
    {
        UpdateSlots();
    }

    public void CreateSlots()
    {
        itemsDisplay = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Items.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemsDisplay.Add(obj, inventory.Items.Items[i]);
        }
    }

    public void UpdateSlots()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> slot in itemsDisplay)
        {
            if (slot.Value.ID >= 0)
            {
                slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = inventory.ItemDatabase.GetItem[slot.Value.ID].UiDisplay;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.Amount == 1 ? "" : slot.Value.Amount.ToString("n0");
            }
            else
            {
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
    
    public void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
        if (itemsDisplay.ContainsKey(obj))
            mouseItem.hoverItem = itemsDisplay[obj];
    }

    public void OnExit(GameObject obj)
    {
        mouseItem.hoverItem = null;
        mouseItem.hoverObj = null;
    }

    public void OnDragStart(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);
        if (itemsDisplay[obj].ID >= 0)
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = inventory.ItemDatabase.GetItem[itemsDisplay[obj].ID].UiDisplay;
            img.raycastTarget = false;
        }

        mouseItem.obj = mouseObject;
        mouseItem.item = itemsDisplay[obj];
    }

    public void OnDragEnd(GameObject obj)
    {
        if(mouseItem.hoverObj)
        {
            inventory.MoveItem(itemsDisplay[obj], itemsDisplay[mouseItem.hoverObj]);
        }
        else
        {
            inventory.RemoveItem(itemsDisplay[obj]);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }

    public void OnDrag(GameObject obj)
    {
        if(mouseItem.obj != null)
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
    }
}

public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}
