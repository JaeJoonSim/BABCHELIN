using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuUI : MonoBehaviour
{
    public RadialMenu radialMenu;
    public KeyCode key = KeyCode.G;

    public Sprite[] sprites;

    public MapController mapController;

    private void Update()
    {
        if (sprites != null)
            radialMenu.SetPieceImageSprites(sprites);

        if (Input.GetKeyDown(key))
        {
            radialMenu.Show();
        }
        else if (Input.GetKeyUp(key))
        {
            int selected = radialMenu.Hide();
            Debug.Log($"Selected : {selected}");

            mapController.SelectMap(selected);
            //if (selected >= 0 && inventory.Items.Items[selected].Item.ID > 0)
            //{
            //    if (inventory.Items.Items[selected].ItemObject.Type != ItemType.Equipment)
            //    {
            //        Debug.Log($"Selected : {inventory.Items.Items[selected].Item.Name} »ç¿ë");
            //        inventory.Items.Items[selected].Amount--;
            //        if (inventory.Items.Items[selected].Amount <= 0)
            //        {
            //            inventory.Items.Items[selected].RemoveItem();
            //        }
            //    }
            //    else
            //    {
            //        equipItemUI.sprite = inventory.Items.Items[selected].ItemObject.UiDisplay;
            //    }
            //}
        }
    }
}