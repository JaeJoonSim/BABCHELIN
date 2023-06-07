using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemObj : MonoBehaviour
{
    private Totem item;
    public string name; 
    void Start()
    {
        Invoke("aa", 5f);
    }

    void aa()
    {
        item = TotemManager.Instance.getTotem();
        name = item.Name;
    }

}
