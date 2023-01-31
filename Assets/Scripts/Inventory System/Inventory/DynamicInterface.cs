using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DynamicInterface : UserInterface
{
    [Tooltip("인벤토리 프리팹")]
    [SerializeField]
    private GameObject inventoryPrefab;
    public GameObject InventoryPrefab
    {
        get { return inventoryPrefab; }
        set { inventoryPrefab = value; }
    }

    public override void CreateSlots()
    {
        itemsDisplay = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < Inventory.Items.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            OnRightClickEvent(obj);

            itemsDisplay.Add(obj, Inventory.Items.Items[i]);
        }
    }
}