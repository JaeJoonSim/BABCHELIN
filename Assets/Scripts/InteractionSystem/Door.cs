using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, Interactable
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
}
