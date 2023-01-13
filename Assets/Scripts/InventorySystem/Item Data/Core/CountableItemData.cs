using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItemData : ItemData
{
    public int MaxAmount => maxAmount;

    [SerializeField]
    [Tooltip("아이템 최대 개수")]
    private int maxAmount = 99;
}
