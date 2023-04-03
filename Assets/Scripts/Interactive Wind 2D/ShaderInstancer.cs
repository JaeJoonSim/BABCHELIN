﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderInstancer : MonoBehaviour
{
    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = renderer.material;
        }
        else
        {
            Graphic graphic = GetComponent<Graphic>();

            if (graphic.material != null)
            {
                graphic.material = Instantiate<Material>(graphic.material);
            }
        }
    }
}