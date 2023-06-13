using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    private Vector2 pivot = new Vector2(-575, -380);
    public List<GameObject> BuffList = new List<GameObject>();

    public GameObject buffUI;
    //RectTransform

    private void Start()
    {
        InvokeRepeating("UpdateBuff", 0f, 0.1f);
    }

    void UpdateBuff()
    {
        for (int i = 0; i < BuffList.Count; i++)
        {
            if (BuffList[i].GetComponent<BuffUIObj>()?.curCoolTime <= 0)
            {
                Destroy(BuffList[i], 0.1f);
                BuffList.RemoveAt(i);
            }
        }

        for (int i = 0; i < BuffList.Count; i++)
        {
            int ypos = (int)(i / 4);

            BuffList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(pivot.x + ((i % 4) * 70), pivot.y + (ypos * 70));
        }
    }

    public void addBuff(float Time, int idx)
    {
        for (int i = 0; i < BuffList.Count; i++)
        {
            BuffUIObj obj = BuffList[i].GetComponent<BuffUIObj>();
            if (obj != null && obj.idx == idx)
            {
                obj.curCoolTime = Time;
                return;
            }
        }

        GameObject ui = Instantiate(buffUI, transform.position, Quaternion.identity, transform);
        ui.GetComponent<BuffUIObj>().SetBuff(Time, idx);
        BuffList.Add(ui);
    }
    public void removeBuff(int idx)
    {
        for (int i = 0; i < BuffList.Count; i++)
        {

            if (BuffList[i].GetComponent<BuffUIObj>()?.idx <= idx)
            {
                Destroy(BuffList[i]);
            }
            BuffList.Clear();
        }
    }
}
