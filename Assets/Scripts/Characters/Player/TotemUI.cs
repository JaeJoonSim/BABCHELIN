using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemUI : MonoBehaviour
{
    private Vector2 pivot = new Vector2(50, -200);
    public List<GameObject> TotemList = new List<GameObject>();

    public GameObject totemUI;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < TotemList.Count; i++)
        {
            int ypos = (int)(i / 2);

            TotemList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(pivot.x + ((i % 2) * 70), pivot.y - (ypos * 70));
        }
    }
    public void addBuff(Totem totem)
    {
        GameObject ui = Instantiate(totemUI, transform.position, Quaternion.identity, transform);
        ui.GetComponent<TotemUIobj>().SetTotem(totem);
        TotemList.Add(ui);
    }
    public void reSetBuff()
    {
        for (int i = 0; i < TotemList.Count; i++)
        {
            Destroy(TotemList[i]);

        }
        TotemList.Clear();
        foreach (var item in TotemManager.Instance.isAdd.Values)
        {

            addBuff(item);
        }
    }
}
