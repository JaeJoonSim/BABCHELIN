using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Item", menuName = "Inventory System/Item/Food", order = 2)]
public class Food : Item
{
    private void Awake()
    {
        Type = ItemType.Food;
    }

}
