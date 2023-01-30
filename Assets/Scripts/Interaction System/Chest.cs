using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : MonoBehaviour, Interactable
{
    
    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    #region Unity Events
    [Space]
    public UnityEvent onInteraction;
    #endregion

    public bool Interact(Interactor interactor)
    {
        onInteraction.Invoke();
        Debug.Log("Interacted with Chest");
        return true;
    }
}
