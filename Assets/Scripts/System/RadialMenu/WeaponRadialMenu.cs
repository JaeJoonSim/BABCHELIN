using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRadialMenu : MonoBehaviour
{
    public RadialMenu radialMenu;
    public KeyCode key = KeyCode.G;


    public Sprite[] sprites;
    public Sprite orginSprite;

    private void Start()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
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
