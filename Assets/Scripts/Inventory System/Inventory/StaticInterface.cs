using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StaticInterface : UserInterface
{
    [Tooltip("¸Þ´º ½½·Ô")]
    [SerializeField]
    private GameObject[] slots;
    public GameObject[] Slots
    { 
        get { return slots; }
        set { slots = value; }
    }

    public override void CreateSlots()
    {
        itemsDisplay = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < Inventory.Items.Items.Length; i++)
        {
            var obj = slots[i];

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemsDisplay.Add(obj, Inventory.Items.Items[i]);
        }
    }
}
