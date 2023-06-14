using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TotemUI : MonoBehaviour
{
    public Vector2 pivot = new Vector2(30, 50);
    public List<GameObject> TotemList = new List<GameObject>();

    public GameObject totemUI;
    [Header("ÅøÆÁ")]
    public GameObject ToolTip;
    public Image uiIcon;
    public TextMeshProUGUI uiName;
    public TextMeshProUGUI uiInfo;

    private void OnEnable()
    {
        reSetBuff();
        for (int i = 0; i < TotemList.Count; i++)
        {
            int ypos = (int)(i / 2);

            TotemList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(pivot.x + ((i % 2) * 70), pivot.y + (ypos * 70));
        }
    }
    public void addBuff(Totem totem)
    {
        GameObject ui = Instantiate(totemUI, transform.position, Quaternion.identity, transform);
        TotemList.Add(ui);
        TotemUIobj obj = ui.GetComponent<TotemUIobj>();
        obj.uiIcon = uiIcon;
        obj.uiName = uiName;
        obj.uiInfo = uiInfo;
        obj.toolTip = ToolTip;
        obj.SetTotem(totem);
        ToolTip.transform.SetSiblingIndex(transform.childCount - 1);

    }
    public void reSetBuff()
    {
        for (int i = 0; i < TotemList.Count; i++)
        {
            Destroy(TotemList[i]);
        }
        TotemList.Clear();
        if (TotemManager.Instance != null)
        {
            foreach (var item in TotemManager.Instance.isAdd.Values)
            {
                addBuff(item);
            }
        }
       
    }
}
