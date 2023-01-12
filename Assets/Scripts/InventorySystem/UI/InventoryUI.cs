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
    [Tooltip("���� ���� ����")]
    private int horizontalSlotCount = 8;
    public int HorizontalSlotCount
    {
        get { return horizontalSlotCount; }
        set { horizontalSlotCount = value; }
    }

    [Range(0, 10)]
    [SerializeField]
    [Tooltip("���� ���� ����")]
    private int verticalSlotCount = 8;
    public int VerticalSlotCount
    {
        get { return verticalSlotCount; }
        set { verticalSlotCount = value; }
    }

    [SerializeField]
    [Tooltip("���� ���� ����")]
    private float slotMargin = 8f;
    public float SlotMargin
    {
        get { return slotMargin; }
        set { slotMargin = value; }
    }

    [SerializeField]
    [Tooltip("�κ��丮 ���� ���� ����")]
    private float contentAreaPadding = 20f;
    public float ContentAreaPadding
    {
        get { return contentAreaPadding; }
        set { contentAreaPadding = value; }
    }

    [Range(32, 64)]
    [SerializeField]
    [Tooltip("���� ũ��")]
    private int slotSize = 64;
    public int SlotSize
    {
        get { return slotSize; }
        set { slotSize = value; }
    }

    [Space]

    [Header("Connected Objects")]
    [SerializeField]
    [Tooltip("���Ե��� ��ġ�� ����")]
    private RectTransform contentAreaRT;
    public RectTransform ContentAreaRT
    {
        get { return contentAreaRT; }
        set { contentAreaRT = value; }
    }

    [SerializeField]
    [Tooltip("���� ������")]
    private GameObject slotUiPrefab;
    public GameObject SlotUiPrefab
    {
        get { return slotUiPrefab; }
        set { slotUiPrefab = value; }
    }

    [Space]

    [Header("Filter Toggles")]
    [SerializeField]
    [Tooltip("���콺 Ŭ�� ���� ����")]
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
    [Tooltip("���� ���� �̸�����")]
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

                beginDragSlot = null;
                beginDragIconTransform = null;
            }
        }
    }

    private void EndDrag()
    {

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

        // ���� ���� ����
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