using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIObj : MonoBehaviour
{
    public float coolTime;
    public float curCoolTime = 5;
    public Image icon;
    public int idx;
    
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
    }
}
