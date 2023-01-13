using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemData : ItemData
{
    public int MaxDurability => maxDurability;

    /// <summary> 최대 내구도 </summary>
    [SerializeField] private int maxDurability = 100;
}
