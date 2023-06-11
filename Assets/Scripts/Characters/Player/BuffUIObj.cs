using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffUIObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float coolTime;
    public float curCoolTime = 5;
    public Image icon;
    public int idx;
    public int count;
    public GameObject toolTip;

    [Header("툴팁 관리")]
    public Image uiIcon;
    public TextMeshProUGUI uiName;
    public TextMeshProUGUI uiInfo;


    // Update is called once per frame
    void Update()   
    {
        curCoolTime -= Time.deltaTime;
        icon.fillAmount = curCoolTime / coolTime;
    }
    public void SetBuff(float Time, int idx)
    {
        coolTime = Time;
        curCoolTime = coolTime;
        this.idx = idx;
        icon.sprite = Resources.Load<Sprite>("Buff/" + idx.ToString());
        uiIcon.sprite = icon.sprite;
        setTooltip();
    }

    void setTooltip()
    {
        switch (idx)
        {
            case 1001:
                uiName.text = "방어막";
                uiInfo.text = "플레이어 주위에 구형 방어막 생성";
                break;
            case 1002:
                uiName.text = "속도 증가";
                uiInfo.text = "속도 증가 +10%";
                break;
            case 1003:
                uiName.text = "충돌 파괴";
                uiInfo.text = "파괴 오브젝트 플레이어 충돌 시 무조건 파괴";
                break;
            case 1004:
                uiName.text = "피해 감소";
                uiInfo.text = "피해량 감소 30%";
                break;
            case 1005:
                uiName.text = "공격력 증가";
                uiInfo.text = "현재 방에서만 공격력 +50%";
                break;
            case 1006:
                uiName.text = "발사 거리 증가";
                uiInfo.text = "기본 탄환의 발사 거리 증가 +100%";
                break;
            case 1007:
                uiName.text = "무제한";
                uiInfo.text = "일정 시간 동안 탄환 소모 없음";
                break;
            case 1008:
                uiName.text = "체력 회복";
                uiInfo.text = "일정 시간 동안 체력 회복, 초당 2씩 회복";
                break;
            case 1009:
                uiName.text = "몬스터 체력 감소";
                uiInfo.text = "현재 방에 존재하는 모든 몬스터 체력 감소 20";
                break;
            default:
                break;
        }
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
