using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuUI : MonoBehaviour
{
    public RadialMenu radialMenu;
    public KeyCode key = KeyCode.G;

    [Tooltip("인벤토리")]
    [SerializeField]
    private Inventory inventory;

    [Tooltip("현재 장착 장비 UI")]
    [SerializeField]
    private Image equipItemUI;

    public Sprite[] sprites;
    public Sprite orginSprite;
    
    private void Update()
    {
        for (int i = 0; i < inventory.Items.Items.Length; i++)
        {
            if(inventory.Items.Items[i].Item.ID >= 0)
                sprites[i] = inventory.ItemDatabase.GetItem[inventory.Items.Items[i].Item.ID].UiDisplay;
            else
                sprites[i] = orginSprite;
        }
        if (sprites != null)
            radialMenu.SetPieceImageSprites(sprites);

        if (Input.GetKeyDown(key))
        {
            radialMenu.Show();
        }
        else if (Input.GetKeyUp(key))
        {
            int selected = radialMenu.Hide();
            if (selected >= 0)
            {
                if (inventory.Items.Items[selected].ItemObject.Type != ItemType.Equipment)
                {
                    Debug.Log($"Selected : {inventory.Items.Items[selected].Item.Name} 사용");
                    inventory.Items.Items[selected].Amount--;
                    if (inventory.Items.Items[selected].Amount <= 0)
                    {
                        inventory.Items.Items[selected].RemoveItem();
                    }
                }
                else
                {
                    equipItemUI.sprite = inventory.Items.Items[selected].ItemObject.UiDisplay;
                }
            }
        }
    }
}