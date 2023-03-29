using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour, Interactable
{
    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    #region Unity Events
    [Space]
    public UnityEvent onInteraction;
    public UnityEvent offInteraction;
    #endregion

    private void Update()
    {
        OffInteract();
    }

    public bool OnInteract(Interactor interactor)
    {
        var intKey = interactor.GetComponent<InteractionKey>();

        if (intKey == null) return false;

        if (intKey.HasKey)
        {
            onInteraction.Invoke();
            Debug.Log("Opening Door!");
            return true;
        }

        Debug.Log("키가 없습니다!");
        return false;
    }

    public void OffInteract()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            offInteraction.Invoke();
            Debug.Log("Close Door");
        }
    }
}
