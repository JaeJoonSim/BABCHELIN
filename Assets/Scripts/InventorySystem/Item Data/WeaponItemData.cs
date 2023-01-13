using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon", menuName = "Inventory System/Item Data/Weaopn", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    [Tooltip("���ݷ�")]
    [SerializeField] private int damage = 1;

    /// <summary> ���ݷ� </summary>
    public int Damage => damage;

    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}
