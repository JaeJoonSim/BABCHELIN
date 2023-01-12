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
    [Tooltip("아이템 고유 ID")]
    private int id;

    [SerializeField]
    [Tooltip("아이템 이름")]
    private string name;

    [Multiline]

    [SerializeField]
    [Tooltip("아이템 설명")]
    private string tooltip;

    [SerializeField]
    [Tooltip("아이템 아이콘")]
    private Sprite iconSprite;

    [SerializeField]
    [Tooltip("바닥에 떨어질 때 생성할 프리팹")]
    private GameObject dropItemPrefab;

    public abstract Item CreateItem();
}
