using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeSystem : MonoBehaviour
{
    [Tooltip("������")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    [Tooltip("�丮�۾���")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    public GameObject RecipeUI;

    private void Update()
    {
        
    }



}