using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class InventoryUI : MonoBehaviour
{
    [Header("Option")]
    [Range(0, 10)]
    [SerializeField]
    [Tooltip("슬롯 가로 개수")]
    private int horizontalSlotCount = 8;
    public int HorizontalSlotCount
    {
        get { return horizontalSlotCount; }
        set { horizontalSlotCount = value; }
    }

    [Range(0, 10)]
    [SerializeField]
    [Tooltip("슬롯 세로 개수")]
    private int verticalSlotCount = 8;
    public int VerticalSlotCount
    {
        get { return verticalSlotCount; }
        set { verticalSlotCount = value; }
    }

    [SerializeField]
    [Tooltip("슬롯 사이 간격")]
    private float slotMargin = 8f;
    public float SlotMargin
    {
        get { return slotMargin; }
        set { slotMargin = value; }
    }

    [SerializeField]
    [Tooltip("인벤토리 영역 내부 여백")]
    private float contentAreaPadding = 20f;
    public float ContentAreaPadding
    {
        get { return contentAreaPadding; }
        set { contentAreaPadding = value; }
    }

    [Range(32, 64)]
    [SerializeField]
    [Tooltip("슬롯 크기")]
    private int slotSize = 64;
    public int SlotSize
    {
        get { return slotSize; }
        set { slotSize = value; }
    }

    [SerializeField]
    [Tooltip("툴팁 보기")]
    private bool showTooltip = true;
    public bool ShowTooltip
    {
        get { return showTooltip; }
        set { showTooltip = value; }
    }

    [SerializeField]
    [Tooltip("하이라이트 표시")]
    private bool showHighlight = true;
    public bool ShowHighlight
    {
        get { return showHighlight; }
        set { showHighlight = value; }
    }

    [Space]

    [Header("Connected Objects")]
    [SerializeField]
    [Tooltip("슬롯들이 위치할 영역")]
    private RectTransform contentAreaRT;
    public RectTransform ContentAreaRT
    {
        get { return contentAreaRT; }
        set { contentAreaRT = value; }
    }

    [SerializeField]
    [Tooltip("슬롯 프리팹")]
    private GameObject slotUiPrefab;
    public GameObject SlotUiPrefab
    {
        get { return slotUiPrefab; }
        set { slotUiPrefab = value; }
    }

    [SerializeField]
    [Tooltip("아이템 정보를 보여줄 툴팁 UI")]
    private ItemTooltipUI itemTooltip;
    public ItemTooltipUI ItemTooltip
    {
        get { return itemTooltip; }
        set { itemTooltip = value; }
    }

    [Space]

    [Header("Filter Toggles")]
    [SerializeField]
    [Tooltip("마우스 클릭 반전 여부")]
    private bool mouseReversed = false;
    public bool MouseReversed
    {
        get { return mouseReversed; }
        set { mouseReversed = value; }
    }

    private Inventory inventory;

    private List<ItemSlotUI> slotUIList = new List<ItemSlotUI>();
    private GraphicRaycaster gr;
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private ItemSlotUI pointerOverSlot;
    private ItemSlotUI beginDragSlot;
    private Transform beginDragIconTransform;

    private int leftClick = 0;
    private int rightClick = 1;

    private Vector3 beginDragIconPoint;
    private Vector3 beginDragCursorPoint;
    private int beginDragSlotSiblingIndex;

#if UNITY_EDITOR
    [Space]

    [Header("Editor Options")]
    [SerializeField]
    private bool showDebug = true;

    [SerializeField]
    [Tooltip("슬롯 생성 미리보기")]
    private bool showPreview = false;

    [Range(0.01f, 1f)]
    [SerializeField]
    private float previewAlpha = 0.1f;

    private List<GameObject> previewSlotGoList = new List<GameObject>();
    private int prevSlotCountPerLine;
    private int prevSlotLineCount;
    private float prevSlotSize;
    private float prevSlotMargin;
    private float prevContentPadding;
    private float prevAlpha;
    private bool prevShow = false;
    private bool prevMouseReversed = false;
#endif

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EditorLog(object message)
    {
        if (!showDebug) return;
        UnityEngine.Debug.Log($"[InventoryUI] {message}");
    }

    private void Awake()
    {
        Init();
        InitSlot();
    }

    private void Update()
    {
        ped.position = Input.mousePosition;
        OnPointerEnterAndExit();

        ShowOrHideItemTooltip();

        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
    }

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        rrList.Clear();
        gr.Raycast(ped, rrList);

        if (rrList.Count == 0) return null;

        return rrList[0].gameObject.GetComponent<T>();
    }

    private void ShowOrHideItemTooltip()
    {
        bool isValid = pointerOverSlot != null && pointerOverSlot.HasItem && pointerOverSlot.IsAccessible && (pointerOverSlot != beginDragSlot);

        if (isValid)
        {
            UpdateTooltipUI(pointerOverSlot);
            itemTooltip.Show();
        }
        else
            itemTooltip.Hide();
    }

    private void UpdateTooltipUI(ItemSlotUI slot)
    {
        if (!slot.IsAccessible || !slot.HasItem)
            return;

        itemTooltip.SetItemInfo(inventory.GetItemData(slot.Index));
        itemTooltip.SetRectPosition(slot.SlotRect);
    }

    private void OnPointerEnterAndExit()
    {
        var prevSlot = pointerOverSlot;

        var curSlot = pointerOverSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (prevSlot == null)
        {
            if (curSlot != null)
            {
                OnCurrentEnter();
            }
        }
        else
        {
            if (curSlot == null)
            {
                OnPrevExit();
            }
            else if (prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        void OnCurrentEnter()
        {
            if (showHighlight)
                curSlot.Highlight(true);
        }
        
        void OnPrevExit()
        {
            prevSlot.Highlight(false);
        }
    }

    private void OnPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (beginDragSlot != null && beginDragSlot.HasItem)
            {
                beginDragIconTransform = beginDragSlot.IconRect.transform;
                beginDragIconPoint = beginDragIconTransform.position;
                beginDragCursorPoint = Input.mousePosition;

                beginDragSlotSiblingIndex = beginDragSlot.transform.GetSiblingIndex();
                beginDragSlot.transform.SetAsLastSibling();

                beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                beginDragSlot = null;
            }
        }

        else if (Input.GetMouseButtonDown(1))
        {
            ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (slot != null && slot.HasItem && slot.IsAccessible)
            {
                TryUseItem(slot.Index);
            }
        }
    }

    private void OnPointerDrag()
    {
        if (beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            beginDragIconTransform.position = beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
        }
    }

    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (beginDragSlot != null)
            {
                beginDragIconTransform.position = beginDragIconPoint;

                beginDragSlot.transform.SetSiblingIndex(beginDragSlotSiblingIndex);

                EndDrag();

                beginDragSlot.SetHighlightOnTop(true);

                beginDragSlot = null;
                beginDragIconTransform = null;
            }
        }
    }

    private void EndDrag()
    {
        ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        // 아이템 슬롯끼리 아이콘 교환 또는 이동
        if (endDragSlot != null && endDragSlot.IsAccessible)
        {
            // 수량 나누기 조건
            // 1) 마우스 클릭 떼는 순간 좌측 Ctrl 또는 Shift 키 유지
            // 2) begin : 셀 수 있는 아이템 / end : 비어있는 슬롯
            // 3) begin 아이템의 수량 > 1
            bool isSeparatable =
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                (inventory.IsCountableItem(beginDragSlot.Index) && !inventory.HasItem(endDragSlot.Index));

            // true : 수량 나누기, false : 교환 또는 이동
            bool isSeparation = false;
            int currentAmount = 0;

            // 현재 개수 확인
            if (isSeparatable)
            {
                currentAmount = inventory.GetCurrentAmount(beginDragSlot.Index);
                if (currentAmount > 1)
                {
                    isSeparation = true;
                }
            }

            // 1. 개수 나누기
            //if (isSeparation)
            //TrySeparateAmount(beginDragSlot.Index, endDragSlot.Index, currentAmount);
            // 2. 교환 또는 이동
            //else
            TrySwapItems(beginDragSlot, endDragSlot);

            // 툴팁 갱신
            //UpdateTooltipUI(endDragSlot);
            return;
        }

        // 버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우)
        if (!IsOverUI())
        {
            // 확인 팝업 띄우고 콜백 위임
            int index = beginDragSlot.Index;
            string itemName = inventory.GetItemName(index);
            int amount = inventory.GetCurrentAmount(index);

            // 셀 수 있는 아이템의 경우, 수량 표시
            if (amount > 1)
                itemName += $" x{amount}";

            // if (_showRemovingPopup)
            //_popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
            //else
            TryRemoveItem(index);
        }
        // 슬롯이 아닌 다른 UI 위에 놓은 경우
        else
        {
            EditorLog($"Drag End(Do Nothing)");
        }

    }

    private void TryRemoveItem(int index)
    {
        inventory.Remove(index);
    }

    private void TryUseItem(int index)
    {
        EditorLog($"UI - Try Use Item : Slot [{index}]");

        inventory.Use(index);
    }

    private bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        if (from == to)
        {
            EditorLog($"UI - Try Swap Items : Same Slot [{from.Index}]");
            return;
        }

        EditorLog($"UI - Try Swap Items : Slot [{from.Index} -> {to.Index}]");

        from.SwapOrMoveIcon(to);

        inventory.Swap(from.Index, to.Index);
    }

    private void Init()
    {
        TryGetComponent(out gr);
        if (gr == null)
            gr = gameObject.AddComponent<GraphicRaycaster>();

        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>(10);

    }

    private void InitSlot()
    {
        slotUiPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);

        slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            slotUiPrefab.AddComponent<ItemSlotUI>();

        slotUiPrefab.SetActive(false);

        Vector2 beginPos = new Vector2(ContentAreaPadding, -contentAreaPadding);
        Vector2 curPos = beginPos;

        slotUIList = new List<ItemSlotUI>(verticalSlotCount * horizontalSlotCount);

        // 슬롯 동적 생성
        for (int i = 0; i < verticalSlotCount; i++)
        {
            for (int j = 0; j < horizontalSlotCount; j++)
            {
                int slotIndex = (horizontalSlotCount * i) + j;

                var slotRT = CloneSlot();
                slotRT.pivot = new Vector2(0f, 1f);
                slotRT.anchoredPosition = curPos;
                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                var slotUI = slotRT.GetComponent<ItemSlotUI>();
                slotUI.SetSlotIndex(slotIndex);
                slotUIList.Add(slotUI);

                curPos.x += (slotMargin + slotSize);
            }

            curPos.x = beginPos.x;
            curPos.y -= (slotMargin + slotSize);
        }

        if (slotUiPrefab.scene.rootCount != 0)
            Destroy(SlotUiPrefab);

        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(slotUiPrefab);
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(contentAreaRT);

            return rt;
        }
    }

    public void SetInventoryReference(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public void InvertMouse(bool value)
    {
        leftClick = value ? 1 : 0;
        rightClick = value ? 0 : 1;

        mouseReversed = value;
    }

    public void SetItemIcon(int index, Sprite icon)
    {
        EditorLog($"Set Item Icon : Slot [{index}]");

        slotUIList[index].SetItem(icon);
    }

    public void SetItemAmountText(int index, int amount)
    {
        EditorLog($"Set Item Amount Text : Slot [{index}], Amount [{amount}]");

        slotUIList[index].SetItemAmount(amount);
    }

    public void HideItemAmountText(int index)
    {
        EditorLog($"Hide Item Amount Text : Slot [{index}]");

        slotUIList[index].SetItemAmount(1);
    }

    public void RemoveItem(int index)
    {
        EditorLog($"Remove Item : Slot [{index}]");

        slotUIList[index].RemoveItem();
    }

    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (prevMouseReversed != mouseReversed)
        {
            prevMouseReversed = mouseReversed;
            InvertMouse(mouseReversed);

            EditorLog($"Mouse Reversed : {mouseReversed}");
        }

        if (Application.isPlaying) return;

        if (showPreview && !prevShow)
        {
            CreateSlots();
        }
        prevShow = showPreview;

        if (Unavailable())
        {
            ClearAll();
            return;
        }

        if (CountChanged())
        {
            ClearAll();
            CreateSlots();
            prevSlotCountPerLine = horizontalSlotCount;
            prevSlotLineCount = verticalSlotCount;
        }

        if (ValueChanged())
        {
            DrawGrid();
            prevSlotSize = slotSize;
            prevSlotMargin = slotMargin;
            prevContentPadding = contentAreaPadding;
        }

        if (AlphaChanged())
        {
            SetImageAlpha();
            prevAlpha = previewAlpha;
        }

        bool Unavailable()
        {
            return !showPreview ||
                    horizontalSlotCount < 1 ||
                    verticalSlotCount < 1 ||
                    slotSize <= 0 ||
                    contentAreaRT == null ||
                    slotUiPrefab == null;
        }

        bool CountChanged()
        {
            return horizontalSlotCount != prevSlotCountPerLine ||
                    verticalSlotCount != prevSlotLineCount;
        }

        bool ValueChanged()
        {
            return slotSize != prevSlotSize ||
                    slotMargin != prevSlotMargin ||
                    contentAreaPadding != prevContentPadding;
        }

        bool AlphaChanged()
        {
            return previewAlpha != prevAlpha;
        }

        void ClearAll()
        {
            foreach (var go in previewSlotGoList)
            {
                Destroyer.Destroy(go);
            }
            previewSlotGoList.Clear();
        }

        void CreateSlots()
        {
            int count = horizontalSlotCount * verticalSlotCount;
            previewSlotGoList.Capacity = count;

            RectTransform slotPrefabRT = slotUiPrefab.GetComponent<RectTransform>();
            slotPrefabRT.pivot = new Vector2(0f, 1f);

            for (int i = 0; i < count; i++)
            {
                GameObject slotGo = Instantiate(slotUiPrefab);
                slotGo.transform.SetParent(contentAreaRT.transform);
                slotGo.SetActive(true);
                slotGo.AddComponent<PreviewItemSlot>();

                slotGo.transform.localScale = Vector3.one;

                HideGameObject(slotGo);

                previewSlotGoList.Add(slotGo);
            }

            DrawGrid();
            SetImageAlpha();
        }

        void DrawGrid()
        {
            Vector2 beginPos = new Vector2(contentAreaPadding, -contentAreaPadding);
            Vector2 curPos = beginPos;

            int index = 0;
            for (int i = 0; i < verticalSlotCount; i++)
            {
                for (int j = 0; j < horizontalSlotCount; j++)
                {
                    GameObject slotGo = previewSlotGoList[index++];
                    RectTransform slotRT = slotGo.GetComponent<RectTransform>();

                    slotRT.anchoredPosition = curPos;
                    slotRT.sizeDelta = new Vector2(slotSize, slotSize);
                    previewSlotGoList.Add(slotGo);

                    curPos.x += (slotMargin + slotSize);
                }

                curPos.x = beginPos.x;
                curPos.y -= (slotMargin + slotSize);
            }
        }

        void HideGameObject(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;

            Transform tr = go.transform;
            for (int i = 0; i < tr.childCount; i++)
            {
                tr.GetChild(1).gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        void SetImageAlpha()
        {
            foreach (var go in previewSlotGoList)
            {
                var images = go.GetComponentsInChildren<Image>();
                foreach (var img in images)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, previewAlpha);
                    var outline = img.GetComponent<Outline>();
                    if (outline)
                        outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, previewAlpha);
                }
            }
        }
    }

    private class PreviewItemSlot : MonoBehaviour { }

    [UnityEditor.InitializeOnLoad]
    private static class Destroyer
    {
        private static Queue<GameObject> targetQueue = new Queue<GameObject>();

        static Destroyer()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                for (int i = 0; targetQueue.Count > 0 && i < 100000; i++)
                {
                    var next = targetQueue.Dequeue();
                    DestroyImmediate(next);
                }
            };
        }
        public static void Destroy(GameObject go) => targetQueue.Enqueue(go);
    }

#endif

}