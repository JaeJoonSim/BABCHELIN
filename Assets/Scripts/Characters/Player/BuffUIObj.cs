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

    [Header("���� ����")]
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
                uiName.text = "��";
                uiInfo.text = "�÷��̾� ������ ���� �� ����";
                break;
            case 1002:
                uiName.text = "�ӵ� ����";
                uiInfo.text = "�ӵ� ���� +10%";
                break;
            case 1003:
                uiName.text = "�浹 �ı�";
                uiInfo.text = "�ı� ������Ʈ �÷��̾� �浹 �� ������ �ı�";
                break;
            case 1004:
                uiName.text = "���� ����";
                uiInfo.text = "���ط� ���� 30%";
                break;
            case 1005:
                uiName.text = "���ݷ� ����";
                uiInfo.text = "���� �濡���� ���ݷ� +50%";
                break;
            case 1006:
                uiName.text = "�߻� �Ÿ� ����";
                uiInfo.text = "�⺻ źȯ�� �߻� �Ÿ� ���� +100%";
                break;
            case 1007:
                uiName.text = "������";
                uiInfo.text = "���� �ð� ���� źȯ �Ҹ� ����";
                break;
            case 1008:
                uiName.text = "ü�� ȸ��";
                uiInfo.text = "���� �ð� ���� ü�� ȸ��, �ʴ� 2�� ȸ��";
                break;
            case 1009:
                uiName.text = "���� ü�� ����";
                uiInfo.text = "���� �濡 �����ϴ� ��� ���� ü�� ���� 20";
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
