using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Portion", menuName = "Inventory System/Item Data/Portion", order = 3)]
public class PortionItemData : CountableItemData
{
    public float Value => value;
    [SerializeField]
    [Tooltip("È¿°ú·®")]
    private float value;

    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}
