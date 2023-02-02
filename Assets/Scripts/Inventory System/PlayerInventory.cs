using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Tooltip("마우스")]
    [SerializeField]
    private MouseItem mouseItem = new MouseItem();
    public MouseItem MouseItem
    {
        get { return mouseItem; }
        set { mouseItem = value; }
    }

    [Tooltip("인벤토리")]
    [SerializeField]
    private Inventory inventory;
    public Inventory Inventory { get { return inventory; } }

    [Tooltip("Radial Menu")]
    [SerializeField]
    private Inventory radialMenu;
    public Inventory RadialMenu { get { return radialMenu; } }

    [Tooltip("쿠킹 테이블")]
    [SerializeField]
    private Inventory cookingTable;
    public Inventory CookingTable { get { return cookingTable; } }

    [Tooltip("오감 촉진제")]
    [SerializeField]
    private Inventory accelerator;
    public Inventory Accelerator { get { return accelerator; } }

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new ItemObject(item.Item), 1);
            Destroy(other.gameObject);
        }
        
    }

    private void Update()
    {
        // Save
        // inventory.Save();

        // Load
        // inventory.Load();
    }

    private void OnApplicationQuit()
    {
        if (inventory != null)
            inventory.Items.Clear();
        if (radialMenu != null)
            radialMenu.Items.Clear();
        if (cookingTable != null)
        {
            cookingTable.Items.Clear();
            for (int i = 0; i < cookingTable.Items.Items.Length; i++)
            {
                cookingTable.Items.Items[i].allowedItems = new ItemType[2];
                cookingTable.Items.Items[i].allowedItems[0] = ItemType.Food;
                cookingTable.Items.Items[i].allowedItems[1] = ItemType.Accelerator;
            }
        }
        
        if (accelerator != null)
        {
            accelerator.Items.Clear();
            for (int i = 0; i < accelerator.Items.Items.Length; i++)
            {
                accelerator.Items.Items[i].allowedItems = new ItemType[1];
                accelerator.Items.Items[i].allowedItems[0] = ItemType.Accelerator;
            }
        }
    }
}
