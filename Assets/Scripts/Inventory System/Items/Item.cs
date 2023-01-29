using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemType
{
    Food,
    Equipment,
    Default,
}

public enum Attributes
{
    Agility,
    Intellect,
    Stamina,
    Strength,
}

public abstract class Item : ScriptableObject
{
    [Tooltip("������ ID")]
    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
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
    [SerializeField, TextArea(15, 20)]
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

    [Tooltip("����")]
    [SerializeField]
    public ItemBuff[] buffs;
    public ItemBuff[] Buffs
    {
        get { return buffs; }
        set { buffs = value; }
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
    private int id;
    public int ID { get { return id; } }

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

    [Tooltip("������ Ÿ��")]
    [SerializeField]
    private ItemType type;
    public ItemType Type { get { return type; } }

    public ItemObject()
    {
        name = "";
        id = -1;
    }

    public ItemObject(Item item)
    {
        name = item.name;
        id = item.ID;
        description = item.Description;
        buffs = new ItemBuff[item.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].Min, item.buffs[i].Max) { Attribute = item.buffs[i].Attribute };
        }
        type = item.Type;
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
