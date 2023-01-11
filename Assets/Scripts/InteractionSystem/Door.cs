using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        var intKey = interactor.GetComponent<InteractionKey>();

        if (intKey == null) return false;

        if (intKey.HasKey)
        {
            Debug.Log("Opening Door!");
            return true;
        }

        Debug.Log("키가 없습니다!");
        return false;
    }
}
