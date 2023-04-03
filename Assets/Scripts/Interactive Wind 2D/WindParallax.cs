﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParallax : MonoBehaviour
{
    float originalXPosition;

    void Awake()
    {
        originalXPosition = transform.position.x;
    }

    void Start()
    {
        GetComponent<Renderer>().material.SetFloat("_WindXPosition", originalXPosition);
    }
}