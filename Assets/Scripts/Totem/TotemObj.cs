using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemObj : MonoBehaviour
{
    private Totem item;
    public string name;

    void Start()
    {
        Invoke("getItem", 3f);
    }

    void getItem()
    {
        item = TotemManager.Instance.getTotem();
        name = item.Name;
    }

    public void setItmeToPlayer()
    {
        TotemManager.Instance.isAdd[item.Type]= item;
        absorb.Instance.Player.gameObject.GetComponent<PlayerController>().addItem();
        
    }
}
