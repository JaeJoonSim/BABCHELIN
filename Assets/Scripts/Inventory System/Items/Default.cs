using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Item", menuName = "Inventory System/Item/Default", order = 1)]
public class Default : Item
{
    private void Awake()
    {
        Type = ItemType.Default;
    }
}
