using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int Capacity { get; private set; }

    [SerializeField, Range(8, 64)]
    [Tooltip("초기 수용 한도")]
    private int initialCapacity = 32;
    public int InitialCapacity
    {
        get { return initialCapacity; }
        set { initialCapacity = value; }
    }

    [SerializeField, Range(8, 64)]
    [Tooltip("최대 수용 한도")]
    private int maxCapacity = 64;
    public int MaxCapacity
    {
        get { return maxCapacity; }
        set { maxCapacity = value; }
    }

    [SerializeField]
    [Tooltip("연결된 인벤토리 UI")]
    private InventoryUI inventoryUI;

    [SerializeField]
    [Tooltip("아이템 목록")]
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

    /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    /// <summary> 앞에서부터 비어있는 슬롯 인덱스 탐색 </summary>
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

            // 아이템 종류 일치, 개수 여유 확인
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

        // 1. 아이템이 슬롯에 존재하는 경우
        if (item != null)
        {
            // 아이콘 등록
            inventoryUI.SetItemIcon(index, item.Data.IconSprite);

            // 1-1. 셀 수 있는 아이템
            if (item is CountableItem ci)
            {
                // 1-1-1. 수량이 0인 경우, 아이템 제거
                if (ci.IsEmpty)
                {
                    items[index] = null;
                    RemoveIcon();
                    return;
                }
                // 1-1-2. 수량 텍스트 표시
                else
                {
                    inventoryUI.SetItemAmountText(index, ci.Amount);
                }
            }
            // 1-2. 셀 수 없는 아이템인 경우 수량 텍스트 제거
            else
            {
                inventoryUI.HideItemAmountText(index);
            }
        }
        // 2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            inventoryUI.RemoveItem(index);
            inventoryUI.HideItemAmountText(index); // 수량 텍스트 숨기기
        }
    }

    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
            UpdateSlot(i);
    }

    private void UpdateAllSlot()
    {
        for (int i = 0; i < Capacity; i++)
            UpdateSlot(i);
    }

    /// <summary> 모든 슬롯 UI에 접근 가능 여부 업데이트 </summary>
    public void UpdateAccessibleStatesAll()
    {
        inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    /// <summary> 해당 슬롯이 아이템을 갖고 있는지 여부 </summary>
    public bool HasItem(int index)
    {
        return IsValidIndex(index) && items[index] != null;
    }

    /// <summary> 해당 슬롯이 셀 수 있는 아이템인지 여부 </summary>
    public bool IsCountableItem(int index)
    {
        return HasItem(index) && items[index] is CountableItem;
    }

    /// <summary>
    /// 해당 슬롯의 현재 아이템 개수 리턴
    /// <para/> - 잘못된 인덱스 : -1 리턴
    /// <para/> - 빈 슬롯 : 0 리턴
    /// <para/> - 셀 수 없는 아이템 : 1 리턴
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

    /// <summary> 해당 슬롯의 아이템 정보 리턴 </summary>
    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (items[index] == null) return null;

        return items[index].Data;
    }

    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (items[index] == null) return "";

        return items[index].Data.Name;
    }

    /// <summary> 해당 슬롯의 아이템 제거 </summary>
    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        items[index] = null;
        inventoryUI.RemoveItem(index);
    }

    /// <summary> 두 인덱스의 아이템 위치를 서로 교체 </summary>
    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item itemA = items[indexA];
        Item ItemB = items[indexB];

        if (itemA != null && ItemB != null && itemA.Data == ItemB.Data && itemA is CountableItem ciA && ItemB is CountableItem ciB)
        {
            int maxAmount = ciB.MaxAmount;
            int sum = ciA.Amount + ciB.Amount;

            if (sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);
                ciB.SetAmount(maxAmount);
            }
        }
        else
        {
            items[indexA] = ItemB;
            items[indexB] = itemA;
        }

        UpdateSlot(indexA, indexB);
    }
}
