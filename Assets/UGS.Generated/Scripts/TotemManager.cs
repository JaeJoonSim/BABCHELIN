using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UGS;
using UnityEngine;


public class TotemManager : BaseMonoBehaviour
{
    public List<Totem> totemSet = new List<Totem>();
    public Dictionary<int, Totem> isAdd = new Dictionary<int, Totem>();
    static private TotemManager instance;
    static public TotemManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            UnityGoogleSheet.Load<DefaultTable.Data>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var x in DefaultTable.Data.DataList)
        {
            if (x.type != 99)
            {
                Debug.Log(x.name);
                Totem tmp = new Totem(x.item, x.type, x.name, x.description, x.stat1, x.val1, x.stat2, x.val2);
                totemSet.Add(tmp);
            }
        }
    }

    public Totem getTotem()
    {
        int idx = UnityEngine.Random.Range(0, totemSet.Count);

        if (!isAdd.ContainsKey(totemSet[idx].Type))
        {
            return totemSet[idx];
        }
        else
        {
            if (isAdd[totemSet[idx].Type].Type == totemSet[idx].Type)
            {
                if (isAdd[totemSet[idx].Type].Item < totemSet[idx].Item)
                {
                    return totemSet[idx];
                }
            }
            else
            {
                return getTotem();
            }
        }
        return getTotem();
    }

}
