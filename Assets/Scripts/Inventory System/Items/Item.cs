using System;
using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("아이템 ID")]
    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    [Tooltip("UI 출력 Sprite")]
    [SerializeField]
    private Sprite uiDisplay;
    public Sprite UiDisplay
    {
        get { return uiDisplay; }
        set { uiDisplay = value; }
    }

    [Tooltip("아이템 타입")]
    [SerializeField]
    private ItemType type;
    public ItemType Type
    {
        get { return type; }
        set { type = value; }
    }

    [Tooltip("아이템 설명")]
    [SerializeField, TextArea(15, 20)]
    private string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    [Tooltip("버프")]
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
    [Tooltip("아이템 이름")]
    [SerializeField]
    private string name;
    public string Name { get { return name; } }

    [Tooltip("아이템 ID")]
    [SerializeField]
    private int id;
    public int ID { get { return id; } }

    [Tooltip("버프")]
    [SerializeField]
    public ItemBuff[] buffs;
    public ItemBuff[] Buffs
    {
        get { return buffs; }
        set { buffs = value; }
    }

    public ItemObject(Item item)
    {
        name = item.name;
        id = item.ID;
        buffs = new ItemBuff[item.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].Min, item.buffs[i].Max) { Attribute = item.buffs[i].Attribute };
        }
    }
}

[System.Serializable]
public class ItemBuff
{
    [Tooltip("버프 타입")]
    [SerializeField]
    private Attributes attribute;
    public Attributes Attribute
    {
        get { return attribute; }
        set { attribute = value; }
    }

    [Tooltip("버프 수치")]
    [SerializeField]
    private int value;
    public int Value
    {
        get { return this.value; }
        set { this.value = value; }
    }

    [Tooltip("버프 최소 수치")]
    [SerializeField]
    private int min;
    public int Min
    {
        get { return min; }
        set { min = value; }
    }

    [Tooltip("버프 최대 수치")]
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
