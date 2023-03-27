using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCommands : CommandBehaviour
{
    [Tooltip("아이템 데이터 베이스")]
    [SerializeField]
    private ItemDatabase itemDatabase;

    [Tooltip("인벤토리")]
    [SerializeField]
    private Inventory inventory;

    protected override void Start()
    {
        base.Start();
    }

    // 아이템 추가
    [Command]
    public void add_item(string name, int amount)
    {
        for (int i = 0; i < itemDatabase.Items.Length; i++)
        {
            if (itemDatabase.Items[i].name == name)
            {
                inventory.AddItem(itemDatabase.Items[i].CreateItem(), amount);
                return;
            }
        }
    }
}
