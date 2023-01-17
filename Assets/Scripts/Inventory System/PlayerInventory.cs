using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Tooltip("인벤토리")]
    [SerializeField]
    private Inventory inventory;
    public Inventory Inventory { get { return inventory; } }

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
        inventory.Items.Items = new InventorySlot[25];
    }
}
