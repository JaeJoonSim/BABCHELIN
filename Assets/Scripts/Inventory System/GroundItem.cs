using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    [Tooltip("������")]
    [SerializeField]
    private Item item;
    public Item Item
    {
        get { return item; }
        set { item = value; }
    }
}
