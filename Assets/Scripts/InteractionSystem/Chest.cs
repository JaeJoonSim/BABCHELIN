using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, Interactable
{
    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Interacted with Chest");
        return true;
    }
}
