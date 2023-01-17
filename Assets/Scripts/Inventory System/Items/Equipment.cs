using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment Item", menuName = "Inventory System/Item/Equipment", order = 3)]
public class Equipment : Item
{
    private void Awake()
    {
        Type = ItemType.Equipment;
    }
}
