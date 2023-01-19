using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    [Tooltip("�����ͺ��̽� ���� ���")]
    [SerializeField]
    private string databasePath;
    public string DatabasePath
    {
        get { return databasePath; }
        set { databasePath = value; }
    }

    [Tooltip("������ ������ ���̽�")]
    [SerializeField]
    private ItemDatabase itemDatabase;
    public ItemDatabase ItemDatabase { get { return itemDatabase; } }

    [Tooltip("�κ��丮")]
    [SerializeField]
    private InventoryObject items;
    public InventoryObject Items { get { return items; } }

    public void AddItem(ItemObject item, int amount)
    {
        if (item.buffs.Length > 0)
        {
            SetEmptySlot(item, amount);
            return;
        }

        for (int i = 0; i < items.Items.Length; i++)
        {
            if (items.Items[i].ID == item.ID && items.Items[i].Amount < itemDatabase.GetItem[item.ID].MaxStack)
            {
                items.Items[i].AddAmount(amount);
                return;
            }
        }
        SetEmptySlot(item, amount);
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
        item2.UpdateSlot(item1.ID, item1.Item, item1.Amount);
        item1.UpdateSlot(temp.ID, temp.Item, temp.Amount);
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
    [Tooltip("������ ����Ʈ")]
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
    [Tooltip("��� ������ Ÿ��")]
    [SerializeField]
    private ItemType[] AllowedItems = new ItemType[0];
    public ItemType[] allowedItems 
    { 
        get { return AllowedItems; }
        set { AllowedItems = value; }
    }

    [Tooltip("User Interface Script")]
    [SerializeField]
    private UserInterface parent;
    public UserInterface Parent 
    { 
        get { return parent; }
        set { parent = value; }
    }

    [Tooltip("������ ID")]
    [SerializeField]
    private int id = -1;
    public int ID 
    { 
        get { return id; }
        set { id = value; }
    }

    [Tooltip("������")]
    [SerializeField]
    private ItemObject item;
    public ItemObject Item
    {
        get { return item; }
        set { item = value; }
    }

    [Tooltip("������ ����")]
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
        if (AllowedItems.Length <= 0)
            return true;

        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (item.Type == AllowedItems[i])
                return true;
        }
        return false;
    }
}
