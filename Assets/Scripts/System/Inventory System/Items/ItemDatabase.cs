using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory System/Database")]
public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("æ∆¿Ã≈€")]
    [SerializeField]
    private Item[] items;
    public Item[] Items { get { return items; } }

    public Dictionary<int, Item> GetItem = new Dictionary<int, Item>();

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < items.Length; i++)
        {
            Items[i].Data.ID = i;
            GetItem.Add(i, items[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        GetItem = new Dictionary<int, Item>();
    }
}
