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
    private Inventory enhancer;
    public Inventory Enhancer { get { return enhancer; } }

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
        if(inventory != null)
            inventory.Items.Items = new InventorySlot[25];
        if (radialMenu != null)
            radialMenu.Items.Items = new InventorySlot[6];
        if (cookingTable != null)
            cookingTable.Items.Items = new InventorySlot[4];
        if (enhancer != null)
            enhancer.Items.Items = new InventorySlot[20];
    }
}
