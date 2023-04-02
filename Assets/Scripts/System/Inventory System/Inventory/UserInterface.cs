using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class UserInterface : MonoBehaviour
{
    [Tooltip("플레이어 인벤토리")]
    [SerializeField]
    private PlayerInventory playerInventory;
    public PlayerInventory PlayerInventory
    {
        get { return playerInventory; }
        set { playerInventory = value; }
    }

    [Tooltip("인벤토리")]
    [SerializeField]
    private Inventory inventory;
    public Inventory Inventory { get { return inventory; } }

    [Tooltip("상호작용할 다른 데이터베이스\n 0번째 인덱스만 우클릭 상호작용 가능")]
    [SerializeField]
    private Inventory[] anotherData;
    public Inventory[] AnotherData { get { return anotherData; } }

    [Tooltip("툴팁")]
    [SerializeField]
    private Tooltip tooltip;
    public Tooltip Tooltip { get { return tooltip; } }

    protected Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();


    private void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        }

        for (int i = 0; i < inventory.Items.Items.Length; i++)
        {
            inventory.Items.Items[i].Parent = this;
        }

        CreateSlots();

        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    private void Update()
    {
        slotsOnInterface.UpdateSlotDisplay();
    }

    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    protected void OnRightClickEvent(GameObject obj)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = EventTriggerType.PointerDown;
        eventTrigger.callback.AddListener((data) => { OnClick(obj, (PointerEventData)data, PointerEventData.InputButton.Right); });
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;

        if (slotsOnInterface.ContainsKey(obj))
        {
            if (slotsOnInterface[obj].Item.ID >= 0)
            {
                tooltip.gameObject.SetActive(true);
                tooltip.SetupTooltip(slotsOnInterface[obj].Item.Name, slotsOnInterface[obj].Item.Description);
            }
        }
    }

    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
        tooltip.gameObject.SetActive(false);
    }

    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnDragStart(GameObject obj)
    {
        if (slotsOnInterface[obj].Item.ID <= -1)
            return;

        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }

    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotsOnInterface[obj].Item.ID >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(gameObject.GetComponentInParent<Canvas>().transform);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.UiDisplay;
            img.raycastTarget = false;
        }
        return tempItem;
    }

    public void OnDragEnd(GameObject obj)
    {
        //else if (playerInventory.MouseItem.hoverItem == null && EventSystem.current.IsPointerOverGameObject() == false)
        //{
        //    inventory.RemoveItem(itemsDisplay[obj]);
        //}

        Destroy(MouseData.tempItemBeingDragged);

        if (MouseData.tempItemBeingDragged == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }

    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void OnClick(GameObject obj, PointerEventData data, PointerEventData.InputButton type)
    {
        if (data.button == type)
        {
            Use(obj);
        }
    }

    public void Use(GameObject obj)
    {
        if (slotsOnInterface[obj].Item.ID >= 0)
        {
            if (slotsOnInterface[obj].ItemObject.Type != ItemType.Equipment)
            {
                slotsOnInterface[obj].Amount--;
                if (slotsOnInterface[obj].Amount <= 0)
                {
                    inventory.RemoveItem(slotsOnInterface[obj]);
                }
            }
            else
            {
                for (int i = 0; i < anotherData[0].Items.Items.Length; i++)
                {
                    if (anotherData[0].Items.Items[i].Item.ID < 0)
                    {
                        inventory.SwapItems(slotsOnInterface[obj], anotherData[0].Items.Items[i]);
                    }
                }
            }
        }
    }
}

public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
}

public static class ExtentionMethods
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, InventorySlot> slot in _slotsOnInterface)
        {
            if (slot.Value.Item.ID >= 0)
            {
                slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = slot.Value.ItemObject.UiDisplay;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                if (slot.Key.GetComponentInChildren<TextMeshProUGUI>())
                    slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.Amount == 1 ? "" : slot.Value.Amount.ToString("n0");
            }
            else
            {
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                if (slot.Key.GetComponentInChildren<TextMeshProUGUI>())
                    slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}
