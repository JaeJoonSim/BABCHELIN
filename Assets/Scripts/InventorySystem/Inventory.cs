using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int Capacity { get; private set; }

    [SerializeField, Range(8, 64)]
    [Tooltip("�ʱ� ���� �ѵ�")]
    private int initialCapacity = 32;
    public int InitialCapacity
    {
        get { return initialCapacity; }
        set { initialCapacity = value; }
    }

    [SerializeField, Range(8, 64)]
    [Tooltip("�ִ� ���� �ѵ�")]
    private int maxCapacity = 64;
    public int MaxCapacity
    {
        get { return maxCapacity; }
        set { maxCapacity = value; }
    }

    [SerializeField]
    [Tooltip("����� �κ��丮 UI")]
    private InventoryUI inventoryUI;

    [SerializeField]
    [Tooltip("������ ���")]
    private Item[] items;

    private void Awake()
    {
        items = new Item[maxCapacity];
        Capacity = initialCapacity;
    }

    private void Start()
    {
        UpdateAccessibleStatesAll();
    }

    /// <summary> �ε����� ���� ���� ���� �ִ��� �˻� </summary>
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    /// <summary> �տ������� ����ִ� ���� �ε��� Ž�� </summary>
    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
            if (items[i] == null)
                return i;
        return -1;
    }

    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
        {
            var current = items[i];
            if (current == null)
                continue;

            // ������ ���� ��ġ, ���� ���� Ȯ��
            if (current.Data == target && current is CountableItem ci)
            {
                if (!ci.IsMax)
                    return i;
            }
        }

        return -1;
    }

    private void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        Item item = items[index];

        // 1. �������� ���Կ� �����ϴ� ���
        if (item != null)
        {
            // ������ ���
            inventoryUI.SetItemIcon(index, item.Data.IconSprite);

            // 1-1. �� �� �ִ� ������
            if (item is CountableItem ci)
            {
                // 1-1-1. ������ 0�� ���, ������ ����
                if (ci.IsEmpty)
                {
                    items[index] = null;
                    RemoveIcon();
                    return;
                }
                // 1-1-2. ���� �ؽ�Ʈ ǥ��
                else
                {
                    inventoryUI.SetItemAmountText(index, ci.Amount);
                }
            }
            // 1-2. �� �� ���� �������� ��� ���� �ؽ�Ʈ ����
            else
            {
                inventoryUI.HideItemAmountText(index);
            }
        }
        // 2. �� ������ ��� : ������ ����
        else
        {
            RemoveIcon();
        }

        // ���� : ������ �����ϱ�
        void RemoveIcon()
        {
            inventoryUI.RemoveItem(index);
            inventoryUI.HideItemAmountText(index); // ���� �ؽ�Ʈ �����
        }
    }

    /// <summary> ��� ���� UI�� ���� ���� ���� ������Ʈ </summary>
    public void UpdateAccessibleStatesAll()
    {
        inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    /// <summary> �ش� ������ �������� ���� �ִ��� ���� </summary>
    public bool HasItem(int index)
    {
        return IsValidIndex(index) && items[index] != null;
    }

    /// <summary> �ش� ������ �� �� �ִ� ���������� ���� </summary>
    public bool IsCountableItem(int index)
    {
        return HasItem(index) && items[index] is CountableItem;
    }

    /// <summary>
    /// �ش� ������ ���� ������ ���� ����
    /// <para/> - �߸��� �ε��� : -1 ����
    /// <para/> - �� ���� : 0 ����
    /// <para/> - �� �� ���� ������ : 1 ����
    /// </summary>
    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (items[index] == null) return 0;

        CountableItem ci = items[index] as CountableItem;
        if (ci == null)
            return 1;

        return ci.Amount;
    }

    /// <summary> �ش� ������ ������ ���� ���� </summary>
    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (items[index] == null) return null;

        return items[index].Data;
    }

    /// <summary> �ش� ������ ������ �̸� ���� </summary>
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (items[index] == null) return "";

        return items[index].Data.Name;
    }
}
