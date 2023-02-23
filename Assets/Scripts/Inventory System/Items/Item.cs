using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemType
{
    Default,
    Accelerator,
    Food,
    Equipment,
}

public enum Attributes
{
    Agility,
    Intellect,
    Stamina,
    Strength,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class Item : ScriptableObject
{
    [Tooltip("������ �̸�")]
    [SerializeField]
    private string title;
    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    [Tooltip("UI ��� Sprite")]
    [SerializeField]
    private Sprite uiDisplay;
    public Sprite UiDisplay
    {
        get { return uiDisplay; }
        set { uiDisplay = value; }
    }

    [Tooltip("������ Ÿ��")]
    [SerializeField]
    private ItemType type;
    public ItemType Type
    {
        get { return type; }
        set { type = value; }
    }

    [Tooltip("������ ����")]
    [SerializeField]
    private string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    [Tooltip("������ ���� ����")]
    [SerializeField]
    private bool stackable;
    public bool Stackable
    {
        get { return stackable; }
        set { stackable = value; }
    }

    [Tooltip("�ִ� ������ ����")]
    [SerializeField, DrawIf("stackable", true)]
    private int maxStack = 100;
    public int MaxStack
    {
        get { return maxStack; }
        set { maxStack = value; }
    }

    [Tooltip("������ ������Ʈ")]
    [SerializeField]
    private ItemObject data = new ItemObject();
    public ItemObject Data
    {
        get { return data; }
        set { data = value; }
    }

    public ItemObject CreateItem()
    {
        ItemObject newItem = new ItemObject(this);
        return newItem;
    }
}

[System.Serializable]
public class ItemObject
{
    [Tooltip("������ �̸�")]
    [SerializeField]
    private string name;
    public string Name { get { return name; } }

    [Tooltip("������ ID")]
    [SerializeField]
    private int id = -1;
    public int ID 
    { 
        get { return id; }
        set { id = value; }
    }

    [Tooltip("������ ����")]
    [SerializeField]
    private int price = 0;
    public int PRICE
    {
        get { return price; }
        set { price = value; }
    }

    [Tooltip("������ ���õ�")]
    [SerializeField]
    private int proficiency = 0;
    public int PROFICIENCY
    {
        get { return proficiency; }
        set { proficiency = value; }
    }

    [Tooltip("������ �ż���")]
    [SerializeField]
    private float freshness = 7200;
    public float FRESHNESS
    {
        get { return freshness; }
        set { freshness = value; }
    }

    [Tooltip("������ ����")]
    [SerializeField, TextArea(15, 20)]
    private string description;
    public string Description { get { return description; } }

    [Tooltip("����")]
    [SerializeField]
    public ItemBuff[] buffs;
    public ItemBuff[] Buffs
    {
        get { return buffs; }
        set { buffs = value; }
    }
    
    public ItemObject()
    {
        name = "";
        id = -1;
        price = 0;
    }

    public ItemObject(Item item)
    {
        name = item.Title;
        id = item.Data.ID;
        description = item.Description;
        price = item.Data.PRICE;
        buffs = new ItemBuff[item.Data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.Data.buffs[i].Min, item.Data.buffs[i].Max) { Attribute = item.Data.buffs[i].Attribute };
        }
    }
}

[System.Serializable]
public class ItemBuff
{
    [Tooltip("���� Ÿ��")]
    [SerializeField]
    private Attributes attribute;
    public Attributes Attribute
    {
        get { return attribute; }
        set { attribute = value; }
    }

    [Tooltip("���� ��ġ")]
    [SerializeField]
    private int value;
    public int Value
    {
        get { return this.value; }
        set { this.value = value; }
    }

    [Tooltip("���� �ּ� ��ġ")]
    [SerializeField]
    private int min;
    public int Min
    {
        get { return min; }
        set { min = value; }
    }

    [Tooltip("���� �ִ� ��ġ")]
    [SerializeField]
    private int max;
    public int Max
    {
        get { return max; }
        set { max = value; }
    }

    public ItemBuff(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}
