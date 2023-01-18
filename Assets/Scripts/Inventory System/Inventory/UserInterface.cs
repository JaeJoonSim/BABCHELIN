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

    [Tooltip("툴팁")]
    [SerializeField]
    private Tooltip tooltip;
    public Tooltip Tooltip { get { return tooltip; } }

    protected Dictionary<GameObject, InventorySlot> itemsDisplay = new Dictionary<GameObject, InventorySlot>();


    private void Start()
    {
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
        UpdateSlots();
    }

    public abstract void CreateSlots();

    public void UpdateSlots()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> slot in itemsDisplay)
        {
            if (slot.Value.ID >= 0)
            {
                slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = inventory.ItemDatabase.GetItem[slot.Value.ID].UiDisplay;
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

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        playerInventory.MouseItem.hoverObj = obj;

        if (itemsDisplay.ContainsKey(obj))
        {
            playerInventory.MouseItem.hoverItem = itemsDisplay[obj];
            if (itemsDisplay[obj].ID >= 0)
            {
                tooltip.gameObject.SetActive(true);
                tooltip.SetupTooltip(itemsDisplay[obj].Item.Name, itemsDisplay[obj].Item.Description);
            }
        }
    }

    public void OnExit(GameObject obj)
    {
        playerInventory.MouseItem.hoverItem = null;
        playerInventory.MouseItem.hoverObj = null;
        tooltip.gameObject.SetActive(false);
    }
    
    public void OnEnterInterface(GameObject obj)
    {
        playerInventory.MouseItem.ui = obj.GetComponent<UserInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {
        playerInventory.MouseItem.ui = null;
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
        playerInventory.MouseItem.obj = mouseObject;
        playerInventory.MouseItem.item = itemsDisplay[obj];
    }

    public void OnDragEnd(GameObject obj)
    {
        var itemOnMouse = playerInventory.MouseItem;
        var mouseHoverItem = itemOnMouse.hoverItem;
        var mouseHoverObj = itemOnMouse.hoverObj;
        var getItemObject = inventory.ItemDatabase.GetItem;

        if (itemOnMouse.ui != null)
        {
            if (mouseHoverObj)
            {
                if (mouseHoverItem.CanPlaceInSlot(getItemObject[itemsDisplay[obj].ID]) &&
                    (mouseHoverItem.Item.ID <= -1) ||
                    (mouseHoverItem.Item.ID >= 0 &&
                    itemsDisplay[obj].CanPlaceInSlot(getItemObject[mouseHoverItem.Item.ID])))
                    inventory.MoveItem(itemsDisplay[obj], mouseHoverItem.Parent.itemsDisplay[itemOnMouse.hoverObj]);
            }
            else if (playerInventory.MouseItem.hoverItem == null && EventSystem.current.IsPointerOverGameObject() == false)
            {
                inventory.RemoveItem(itemsDisplay[obj]);
            }
        }        Destroy(itemOnMouse.obj);
        itemOnMouse.item = null;
    }

    public void OnDrag(GameObject obj)
    {
        if (playerInventory.MouseItem.obj != null)
            playerInventory.MouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
    }
}

public class MouseItem
{
    public UserInterface ui;
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}