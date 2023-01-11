using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionKey : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Ű ����")]
    private bool hasKey = false;
    public bool HasKey
    {
        get { return hasKey; }
        set { hasKey = value; }
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) hasKey = !hasKey;
    }
}
