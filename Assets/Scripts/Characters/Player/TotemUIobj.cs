using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class TotemUIobj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public string Name;
    public string Info;
    [Header("ÅøÆÁ °ü¸®")]
    public Image uiIcon;
    public TextMeshProUGUI uiName;
    public TextMeshProUGUI uiInfo;
    public GameObject toolTip;

    public void SetTotem(Totem totem)
    {
        icon.sprite = TotemManager.Instance.Icons[totem.Item];
        Name = totem.Name;
        Info = totem.Description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("in");
        uiIcon.sprite = icon.sprite;
        uiName.text = Name;
        uiInfo.text = Info;
        toolTip.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }
}
