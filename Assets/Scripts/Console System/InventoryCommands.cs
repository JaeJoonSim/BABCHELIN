using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCommands : CommandBehaviour
{
    [Tooltip("������ ������ ���̽�")]
    [SerializeField]
    private ItemDatabase itemDatabase;

    [Tooltip("�κ��丮")]
    [SerializeField]
    private Inventory inventory;

    protected override void Start()
    {
        base.Start();
    }

    // ������ �߰�
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
