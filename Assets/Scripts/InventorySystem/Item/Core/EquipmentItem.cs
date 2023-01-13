using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItem : Item
{
    public EquipmentItemData EquipmentData { get; private set; }

    private int durability;

    public int Durability
    {
        get => durability;
        set
        {
            if (value <= 0) value = 0;
            if (value > EquipmentData.MaxDurability)
                value = EquipmentData.MaxDurability;

            durability = value;
        }
    }
    
    public EquipmentItem(EquipmentItemData data) : base(data)
    {
        EquipmentData = data;
        Durability = data.MaxDurability;
    }
}