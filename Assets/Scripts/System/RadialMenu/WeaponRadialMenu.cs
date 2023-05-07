using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRadialMenu : MonoBehaviour
{
    public RadialMenu radialMenu;

    public Sprite[] addSprites;
    public Sprite[] sprites;
    public Sprite orginSprite;

    private void Start(Transform player)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (addSprites[i] != null)
                sprites[i] = addSprites[i];
            else
                sprites[i] = orginSprite;
        }
        if (sprites != null)
            radialMenu.SetPieceImageSprites(sprites);
    }

    public void Show()
    {
        radialMenu.Show();
    }
    public int Hide()
    {
        return radialMenu.Hide();
    }
}
