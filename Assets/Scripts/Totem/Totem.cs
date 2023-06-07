using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Totem
{
    public int Item;
    public int Type;
    public string Name;
    public string Description;
    public string Stat1;
    public float Val1;
    public string Stat2;
    public float Val2;

    public Totem(int Item, int type, string name, string description, string stat1, float val1, string stat2, float val2)
    {
        this.Item = Item;
        this.Type = type;
        this.Name = name;
        this.Description = description;
        this.Stat1 = stat1;
        this.Val1 = val1;
        this.Stat2 = stat2;
        this.Val2 = val2;
    }
}
