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

    // ? �� getter�� stackoverflow? ��ü ��?
    [Tooltip("�κ��丮")]
    [SerializeField]
    private InventoryObject items;
    public InventoryObject Items { get { return items; } }

    public void AddItem(ItemObject item, int amount)
    {
        if(item.buffs.Length > 0)
        {
            items.Items.Add(new InventorySlot(item.ID, item, amount));
            return;
        }

        for (int i = 0; i < items.Items.Count; i++)
        {
            if (items.Items[i].Item.ID == item.ID && items.Items[i].Amount < 101)
            {
                items.Items[i].AddAmount(amount);
                return;
            }
        }
        items.Items.Add(new InventorySlot(item.ID, item, amount));
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
            items = (InventoryObject)formatter.Deserialize(stream);
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        items = new InventoryObject();
    }
}

[System.Serializable]
public class InventoryObject
{
    [Tooltip("������ ����Ʈ")]
    [SerializeField]
    private List<InventorySlot> items = new List<InventorySlot>();
    public List<InventorySlot> Items { get { return items; } }
}


[System.Serializable]
public class InventorySlot
{
    [Tooltip("������ ID")]
    [SerializeField]
    private int id;
    public int ID { get { return id; } }

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

    public InventorySlot(int id, ItemObject item, int amount)
    {
        this.id = id;
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}
