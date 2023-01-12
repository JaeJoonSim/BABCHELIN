using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int ID => id;
    public string Name => name;
    public string Tooltip => tooltip;
    public Sprite IconSprite => iconSprite;

    [SerializeField] 
    [Tooltip("������ ���� ID")]
    private int id;

    [SerializeField]
    [Tooltip("������ �̸�")]
    private string name;

    [Multiline]

    [SerializeField]
    [Tooltip("������ ����")]
    private string tooltip;

    [SerializeField]
    [Tooltip("������ ������")]
    private Sprite iconSprite;

    [SerializeField]
    [Tooltip("�ٴڿ� ������ �� ������ ������")]
    private GameObject dropItemPrefab;

    public abstract Item CreateItem();
}
