using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UGS;



public class TotemManager : BaseMonoBehaviour
{
    public List<Totem> totemSet = new List<Totem>();
    public Dictionary<int, Totem> isAdd = new Dictionary<int, Totem>();
    static private TotemManager instance;
    static public TotemManager Instance { get { return instance; } }

    void Awake()
    {
        UnityGoogleSheet.LoadFromGoogle<int, DefaultTable.Data>((list, map) =>
        {
            list.ForEach(x =>
            {
                if (x.type != 99)
                {
                    Totem tmp = new Totem(x.item, x.type, x.name, x.description, x.stat1, x.val1, x.stat2, x.val2);
                    totemSet.Add(tmp);
                }

            });
        }, true);
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
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
