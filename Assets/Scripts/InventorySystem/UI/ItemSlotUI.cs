using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("슬롯 내에서 아이콘과 슬롯 사이의 여백")]
    private float padding = 1f;
    public float Padding
    {
        get { return padding; }
        set { padding = value; }
    }

    [SerializeField]
    [Tooltip("아이템 아이콘 이미지")]
    private Image iconImage;
    public Image IconImage
    {
        get { return iconImage; }
        set { iconImage = value; }
    }

    [SerializeField]
    [Tooltip("아이템 개수 텍스트")]
    private TextMeshProUGUI amountText;
    public TextMeshProUGUI AmountText
    {
        get { return amountText; }
        set { amountText = value; }
    }

    [SerializeField]
    [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
    private Image highlightImage;
    public Image HighlightImage
    {
        get { return highlightImage; }
        set { highlightImage = value; }
    }

    [SerializeField]
    [Tooltip("하이라이트 이미지 알파 값")]
    private float highlightAlpha = 0.5f;
    public float HighlightAlpha
    {
        get { return highlightAlpha; }
        set { highlightAlpha = value; }
    }

    [SerializeField]
    [Tooltip("하이라이트 소요 시간")]
    private float highlightFadeDuration = 0.2f;
    public float HighlightFadeDuration
    {
        get { return highlightFadeDuration; }
        set { highlightFadeDuration = value; }
    }

    private InventoryUI inventoryUI;

    private RectTransform slotRect;
    private RectTransform iconRect;
    private RectTransform highlightRect;

    private GameObject iconGo;
    private GameObject textGo;
    private GameObject highlightGo;

    private Image slotImage;

    private float currentHLAlpha = 0f;

    private bool isAccessibleSlot = true;
    private bool isAccessibleItem = true;

    /// <summary> 슬롯의 인덱스 </summary>
    public int Index { get; private set; }

    /// <summary> 슬롯이 아이템을 보유하고 있는지 여부 </summary>
    public bool HasItem => iconImage.sprite != null;

    public bool IsAccessible => isAccessibleSlot && isAccessibleItem;

    public RectTransform SlotRect => slotRect;
    public RectTransform IconRect => iconRect;

    /// <summary> 비활성화된 슬롯의 색상 </summary>
    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    /// <summary> 비활성화된 아이콘 색상 </summary>
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void Awake()
    {
        InitComponents();
        InitValues();
    }

    private void InitComponents()
    {
        inventoryUI = GetComponentInParent<InventoryUI>();

        slotRect = GetComponent<RectTransform>();
        iconRect = iconImage.rectTransform;
        highlightRect = highlightImage.rectTransform;

        iconGo = iconRect.gameObject;
        textGo = amountText.gameObject;
        highlightGo = highlightImage.gameObject;

        slotImage = GetComponent<Image>();
    }

    private void InitValues()
    {
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;

        iconRect.offsetMin = Vector2.one * padding;
        iconRect.offsetMax = Vector2.one * -padding;

        highlightRect.pivot = iconRect.pivot;
        highlightRect.anchorMin = iconRect.anchorMin;
        highlightRect.anchorMax = iconRect.anchorMax;
        highlightRect.offsetMin = iconRect.offsetMin;
        highlightRect.offsetMax = iconRect.offsetMax;

        iconImage.raycastTarget = false;
        highlightImage.raycastTarget = false;

        HideIcon();
        highlightGo.SetActive(false);
    }

    private void ShowIcon() => iconGo.SetActive(true);
    private void HideIcon() => iconGo.SetActive(false);

    private void ShowText() => textGo.SetActive(true);
    private void HideText() => textGo.SetActive(false);

    public void SetSlotIndex(int index) => Index = index;

    /// <summary> 슬롯 자체의 활성화/비활성화 여부 설정 </summary>
    public void SetSlotAccessibleState(bool value)
    {
        if (isAccessibleSlot == value) return;

        if (value)
        {
            slotImage.color = Color.black;
        }
        else
        {
            slotImage.color = InaccessibleSlotColor;
            HideIcon();
            HideText();
        }

        isAccessibleSlot = value;
    }

    /// <summary> 아이템 활성화/비활성화 여부 설정 </summary>
    public void SetItemAccessibleState(bool value)
    {
        if (isAccessibleItem == value) return;

        if (value)
        {
            iconImage.color = Color.white;
            amountText.color = Color.white;
        }
        else
        {
            iconImage.color = InaccessibleIconColor;
            amountText.color = InaccessibleIconColor;
        }

        isAccessibleItem = value;
    }

    /// <summary> 다른 슬롯과 아이템 아이콘 교환 </summary>
    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null) return;
        if (other == this) return;
        if (!this.IsAccessible) return;
        if (!other.IsAccessible) return;

        var temp = iconImage.sprite;

        if (other.HasItem) SetItem(other.iconImage.sprite);

        else RemoveItem();

        other.SetItem(temp);
    }

    public void SetItem(Sprite sprite)
    {
        if (sprite != null)
        {
            iconImage.sprite = sprite;
            ShowIcon();
        }
        else
        {
            RemoveItem();
        }
    }

    public void RemoveItem()
    {
        iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    public void SetIconAlpha(float alpha)
    {
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, alpha);
    }

    public void SetItemAmount(int amount)
    {
        if (HasItem && amount > 1)
            ShowText();
        else
            HideText();

        amountText.text = amount.ToString();
    }

    public void Highlight(bool show)
    {
        if (!this.IsAccessible) return;

        if (show)
            StartCoroutine(nameof(HighlightFadeInRoutine));
        else
            StartCoroutine(nameof(HighlightFadeOutRoutine));
    }

    public void SetHighlightOnTop(bool value)
    {
        if (value)
            highlightRect.SetAsLastSibling();
        else
            highlightRect.SetAsFirstSibling();
    }

    /// <summary> 하이라이트 알파값 서서히 증가 </summary>
    private IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        highlightGo.SetActive(true);

        float unit = highlightAlpha / highlightFadeDuration;

        for (; currentHLAlpha <= highlightAlpha; currentHLAlpha += unit * Time.deltaTime)
        {
            highlightImage.color = new Color(
                highlightImage.color.r,
                highlightImage.color.g,
                highlightImage.color.b,
                currentHLAlpha
            );

            yield return null;
        }
    }

    /// <summary> 하이라이트 알파값 0%까지 서서히 감소 </summary>
    private IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = highlightAlpha / highlightFadeDuration;

        for (; currentHLAlpha >= 0f; currentHLAlpha -= unit * Time.deltaTime)
        {
            highlightImage.color = new Color(
                highlightImage.color.r,
                highlightImage.color.g,
                highlightImage.color.b,
                currentHLAlpha
            );

            yield return null;
        }

        highlightGo.SetActive(false);
    }
}
