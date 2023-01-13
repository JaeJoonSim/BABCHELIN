using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Armor", menuName = "Inventory System/Item Data/Armor", order = 2)]
public class ArmorItemData : EquipmentItemData
{
    public int Defence => defence;

    /// <summary> ¹æ¾î·Â </summary>
    [SerializeField] private int defence = 1;

    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }
}
