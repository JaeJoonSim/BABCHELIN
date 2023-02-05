using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    [Tooltip("데이터베이스 저장 경로")]
    [SerializeField]
    private string databasePath;
    public string DatabasePath
    {
        get { return databasePath; }
        set { databasePath = value; }
    }

    [Tooltip("아이템 데이터 베이스")]
    [SerializeField]
    private ItemDatabase itemDatabase;
    public ItemDatabase ItemDatabase { get { return itemDatabase; } }

    [Tooltip("인벤토리")]
    [SerializeField]
    private InventoryObject items;
    public InventoryObject Items { get { return items; } }

    public bool AddItem(ItemObject item, int amount)
    {
        if (EmptySlotCount == false)
            return false;
        InventorySlot slot = FindItemOnInventory(item);
        if (!itemDatabase.Items[item.ID].Stackable || slot == null)
        {
            SetEmptySlot(item, amount);
            return true;
        }
        if (itemDatabase.Items[item.ID].Stackable && slot.Amount < itemDatabase.Items[item.ID].MaxStack)
        {
            slot.AddAmount(amount);
            return true;
        }
        else if (itemDatabase.Items[item.ID].Stackable && slot.Amount >= itemDatabase.Items[item.ID].MaxStack)
        {
            SetEmptySlot(item, amount);
            return true;
        }
        return true;
    }

    public bool EmptySlotCount
    {
        get
        {
            bool counter = false;
            for (int i = 0; i < items.Items.Length; i++)
            {
                if (items.Items[i].ID <= -1)
                {
                    counter = true;
                }
            }
            return counter;
        }
    }
    public InventorySlot FindItemOnInventory(ItemObject item)
    {
        for (int i = 0; i < items.Items.Length; i++)
        {
            if (items.Items[i].ID == item.ID)
            {
                return items.Items[i];
            }
        }
        return null;
    }

    public InventorySlot SetEmptySlot(ItemObject item, int amount)
    {
        for (int i = 0; i < Items.Items.Length; i++)
        {
            if (Items.Items[i].ID <= -1)
            {
                Items.Items[i].UpdateSlot(item.ID, item, amount);
                return Items.Items[i];
            }
        }
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.ID, item2.Item, item2.Amount);

        if (item1.Item.ID == temp.ID && item1 != item2 && itemDatabase.GetItem[item1.ID].Stackable)
        {
            item2.AddAmount(item1.Amount);
            //item1.UpdateSlot(-1, null, 0);
            if (item2.Amount > itemDatabase.GetItem[item2.ID].MaxStack)
            {
                int amount = item2.Amount - itemDatabase.GetItem[item2.ID].MaxStack;
                item2.Amount = itemDatabase.GetItem[item2.ID].MaxStack;
                item1.UpdateSlot(temp.ID, temp.Item, amount);
            }
        }
        else
        {
            item2.UpdateSlot(item1.ID, item1.Item, item1.Amount);
            item1.UpdateSlot(temp.ID, temp.Item, temp.Amount);
        }
    }

    public void RemoveItem(InventorySlot item)
    {
        for (int i = 0; i < items.Items.Length; i++)
        {
            if (items.Items[i].ID == item.ID)
            {
                items.Items[i].UpdateSlot(-1, null, 0);
            }
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, saveData));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, databasePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, items);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, databasePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, databasePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, databasePath), FileMode.Open, FileAccess.Read);
            InventoryObject newItems = (InventoryObject)formatter.Deserialize(stream);
            for (int i = 0; i < items.Items.Length; i++)
            {
                items.Items[i].UpdateSlot(newItems.Items[i].ID, newItems.Items[i].Item, newItems.Items[i].Amount);
            }
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        items.Clear();
    }
}

[System.Serializable]
public class InventoryObject
{
    [Tooltip("아이템 리스트")]
    [SerializeField]
    private InventorySlot[] items = new InventorySlot[25];
    public InventorySlot[] Items
    {
        get { return items; }
        set { items = value; }
    }

    public void Clear()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].UpdateSlot(-1, new ItemObject(), 0);
        }
    }
}


[System.Serializable]
public class InventorySlot
{
    [Tooltip("허용 아이템 타입")]
    [SerializeField]
    private ItemType[] AllowedItems = new ItemType[0];
    public ItemType[] allowedItems
    {
        get { return AllowedItems; }
        set { AllowedItems = value; }
    }

    private UserInterface parent;
    public UserInterface Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    [Tooltip("아이템 ID")]
    [SerializeField]
    private int id = -1;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    [Tooltip("아이템")]
    [SerializeField]
    private ItemObject item;
    public ItemObject Item
    {
        get { return item; }
        set { item = value; }
    }

    [Tooltip("아이템 개수")]
    [SerializeField]
    private int amount;
    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }

    public InventorySlot()
    {
        id = -1;
        item = null;
        amount = 0;
    }

    public InventorySlot(int id, ItemObject item, int amount)
    {
        this.id = id;
        this.item = item;
        this.amount = amount;
    }

    public void UpdateSlot(int id, ItemObject item, int amount)
    {
        this.id = id;
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public bool CanPlaceInSlot(Item item)
    {
        if (AllowedItems.Length <= 0 || item == null || item.ID < 0)
            return true;

        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (item.Type == AllowedItems[i])
                return true;
        }
        return false;
    }
}
