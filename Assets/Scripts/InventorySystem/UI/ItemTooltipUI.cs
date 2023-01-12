using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("아이템 이름 텍스트")]
    private TextMeshProUGUI titleText;
    public TextMeshProUGUI TitleText
    {
        get { return titleText; }
        set { titleText = value; }
    }

    [SerializeField]
    [Tooltip("아이템 설명 텍스트")]
    private TextMeshProUGUI contentText;
    public TextMeshProUGUI ContentText
    {
        get { return contentText; }
        set { contentText = value; }
    }

    private RectTransform rt;
    private CanvasScaler canvasScaler;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private void Awake()
    {
        Init();
        Hide();
    }

    private void Init()
    {
        TryGetComponent(out rt);
        rt.pivot = new Vector2(0f, 1f);
        canvasScaler = GetComponentInParent<CanvasScaler>();

        DisableAllChildrenRayCastTarget(transform);
    }
    
    private void DisableAllChildrenRayCastTarget(Transform tr)
    {
        tr.TryGetComponent(out Graphic gr);
        if (gr != null)
            gr.raycastTarget = false;

        int childCount = tr.childCount;
        if (childCount == 0) return;

        for(int i = 0; i < childCount; i++)
        {
            DisableAllChildrenRayCastTarget(tr.GetChild(i));
        }
    }
    
    public void SetItemInfo(ItemData data)
    {
        titleText.text = data.Name;
        contentText.text = data.Tooltip;
    }

    public void SetRectPosition(RectTransform slotRect)
    {
        float wRatio = Screen.width / canvasScaler.referenceResolution.x;
        float hRatio = Screen.height / canvasScaler.referenceResolution.y;
        float ratio = wRatio * (1f - canvasScaler.matchWidthOrHeight) + hRatio * (canvasScaler.matchWidthOrHeight);

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        rt.position = slotRect.position + new Vector3(slotWidth, -slotHeight);
        Vector2 pos = rt.position;

        float width = rt.rect.width * ratio;
        float height = rt.rect.height * ratio;

        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        ref bool R = ref rightTruncated;
        ref bool B = ref bottomTruncated;
        
        if(R && !B)
        {
            rt.position = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        else if (!R && B)
        {
            rt.position = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        else if (R && B)
        {
            rt.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
    }
}
