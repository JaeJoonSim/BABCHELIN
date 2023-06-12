using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TotemUIobj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    [Header("ÅøÆÁ °ü¸®")]
    public Image uiIcon;
    public TextMeshProUGUI uiName;
    public TextMeshProUGUI uiInfo;
    public GameObject toolTip;

    public void SetTotem(Totem totem)
    {
        icon.sprite = TotemManager.Instance.Icons[totem.Item];
        uiIcon.sprite = icon.sprite;
        uiName.text = totem.Name;
        uiInfo.text = totem.Description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }
}
