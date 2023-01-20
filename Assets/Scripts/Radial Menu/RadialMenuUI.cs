using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuUI : MonoBehaviour
{
    public RadialMenu radialMenu;
    public KeyCode key = KeyCode.G;

    [Tooltip("�κ��丮")]
    [SerializeField]
    private Inventory inventory;

    public Sprite[] sprites;

    private void Update()
    {
        for (int i = 0; i < inventory.Items.Items.Length; i++)
        {
            if(inventory.Items.Items[i].ID >= 0)
                sprites[i] = inventory.ItemDatabase.GetItem[inventory.Items.Items[i].ID].UiDisplay;
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
            Debug.Log($"Selected : {selected}");
        }
    }
}