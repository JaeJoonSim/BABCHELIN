using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TotemObj : MonoBehaviour
{
    private Totem item;
    public string name;

    public TextMeshProUGUI text;

    void Start()
    {
        Invoke("getItem", 3f);
    }

    void getItem()
    {
        item = TotemManager.Instance.getTotem();
        name = item.Name;

        text.text = item.Name + item.Description;
    }

    public void setItmeToPlayer()
    {
        TotemManager.Instance.isAdd[item.Type]= item;
        absorb.Instance.Player.gameObject.GetComponent<PlayerController>().addItem();
        
    }
}
